using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FreeSql;
using LinCms.Application.Contracts.Blog.Tags;
using LinCms.Application.Contracts.Blog.Tags.Dtos;
using LinCms.Application.Contracts.Blog.UserSubscribes;
using LinCms.Application.Contracts.Blog.UserSubscribes.Dtos;
using LinCms.Core.Data;
using LinCms.Core.Entities.Blog;
using LinCms.Core.Exceptions;
using LinCms.Core.Extensions;
using LinCms.Core.IRepositories;
using LinCms.Core.Security;

namespace LinCms.Application.Blog.Tags
{
    public class TagService : ITagService
    {
        private readonly IAuditBaseRepository<UserTag> _userTagRepository;
        private readonly IAuditBaseRepository<Tag> _tagRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        private readonly BaseRepository<TagArticle> _tagArticleRepository;
        public TagService(IAuditBaseRepository<Tag> tagRepository, IMapper mapper, ICurrentUser currentUser, IAuditBaseRepository<UserTag> userTagRepository, BaseRepository<TagArticle> tagArticleRepository)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
            _currentUser = currentUser;
            _userTagRepository = userTagRepository;
            _tagArticleRepository = tagArticleRepository;
        }

        public async Task CreateAsync(CreateUpdateTagDto createTag)
        {
            bool exist = await _tagRepository.Select.AnyAsync(r => r.TagName == createTag.TagName);
            if (exist)
            {
                throw new LinCmsException($"标签[{createTag.TagName}]已存在");
            }

            Tag tag = _mapper.Map<Tag>(createTag);
            await _tagRepository.InsertAsync(tag);
        }

        public async Task UpdateAsync(Guid id, CreateUpdateTagDto updateTag)
        {
            Tag tag = await _tagRepository.Select.Where(r => r.Id == id).ToOneAsync();
            if (tag == null)
            {
                throw new LinCmsException("该数据不存在");
            }

            bool exist = await _tagRepository.Select.AnyAsync(r => r.TagName == updateTag.TagName && r.Id != id);
            if (exist)
            {
                throw new LinCmsException($"标签[{updateTag.TagName}]已存在");
            }

            _mapper.Map(updateTag, tag);
            await _tagRepository.UpdateAsync(tag);
        }

        public async Task<TagListDto> GetAsync(Guid id)
        {
            Tag tag = await _tagRepository.Select.Where(a => a.Id == id).ToOneAsync();
            if (tag == null)
            {
                throw new LinCmsException("不存在此标签");
            }
            TagListDto tagDto = _mapper.Map<TagListDto>(tag);
            tagDto.IsSubscribe = await this.IsSubscribeAsync(id);
            tagDto.ThumbnailDisplay = _currentUser.GetFileUrl(tagDto.Thumbnail);
            return tagDto;
        }

        /// <summary>
        /// 根据状态得到标签列表
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<TagListDto>> GetListAsync(TagSearchDto searchDto)
        {
            if (searchDto.Sort.IsNullOrEmpty())
            {
                searchDto.Sort = "create_time desc";
            }

            List<TagListDto> tags = (await _tagRepository.Select.IncludeMany(r => r.UserTags, r => r.Where(u => u.CreateUserId == _currentUser.Id))
                        .WhereIf(searchDto.TagIds.IsNotNullOrEmpty(), r => searchDto.TagIds.Contains(r.Id))
                        .WhereIf(searchDto.TagName.IsNotNullOrEmpty(), r => r.TagName.Contains(searchDto.TagName))
                        .WhereIf(searchDto.Status != null, r => r.Status == searchDto.Status)
                        .OrderBy(searchDto.Sort)
                        .ToPagerListAsync(searchDto, out long totalCount))
                        .Select(r =>
                        {
                            TagListDto tagDto = _mapper.Map<TagListDto>(r);
                            tagDto.ThumbnailDisplay = _currentUser.GetFileUrl(tagDto.Thumbnail);
                            tagDto.IsSubscribe = r.UserTags.Any();
                            return tagDto;
                        }).ToList();

            return new PagedResultDto<TagListDto>(tags, totalCount);
        }

        public async Task<bool> IsSubscribeAsync(Guid tagId)
        {
            if (_currentUser.Id == null) return false;
            return await _userTagRepository.Select.AnyAsync(r => r.TagId == tagId && r.CreateUserId == _currentUser.Id);
        }

        public PagedResultDto<TagListDto> GetSubscribeTags(UserSubscribeSearchDto userSubscribeDto)
        {
            var userTags = _userTagRepository.Select.Include(r => r.Tag)
                .Where(r => r.CreateUserId == userSubscribeDto.UserId)
                .OrderByDescending(r => r.CreateTime)
                .ToPagerList(userSubscribeDto, out long count)
                .Select(r =>
                {
                    TagListDto tagDto = _mapper.Map<TagListDto>(r.Tag);
                    tagDto.ThumbnailDisplay = _currentUser.GetFileUrl(tagDto.Thumbnail);
                    tagDto.IsSubscribe = true;
                    return tagDto;
                }).ToList();

            return new PagedResultDto<TagListDto>(userTags, count);
        }

        public void UpdateArticleCount(Guid? id, int inCreaseCount)
        {
            if (id == null)
            {
                return;
            }

            //防止数量一直减，减到小于0
            if (inCreaseCount < 0)
            {
                Tag tag = _tagRepository.Select.Where(r => r.Id == id).ToOne();
                if (tag.ArticleCount < -inCreaseCount)
                {
                    return;
                }
            }

            _tagRepository.UpdateDiy.Set(r => r.ArticleCount + inCreaseCount)
                .Where(r => r.Id == id)
                .ExecuteAffrows();
        }


        public void UpdateSubscribersCount(Guid? id, int inCreaseCount)
        {
            if (id == null)
            {
                return;
            }

            //防止数量一直减，减到小于0
            if (inCreaseCount < 0)
            {
                Tag tag = _tagRepository.Select.Where(r => r.Id == id).ToOne();
                if (tag.SubscribersCount < -inCreaseCount)
                {
                    return;
                }
            }

            _tagRepository.UpdateDiy.Set(r => r.SubscribersCount + inCreaseCount)
                .Where(r => r.Id == id)
                .ExecuteAffrows();
        }
        public async Task CorrectedTagCountAsync(Guid tagId)
        {
            long count = await _tagArticleRepository.Select.Where(r => r.TagId == tagId && r.Article.IsDeleted == false).CountAsync();
            await _tagRepository.UpdateDiy.Set(r => r.ArticleCount, count).Where(r => r.Id == tagId).ExecuteAffrowsAsync();
        }

        public async Task IncreaseTagViewHits(Guid tagId)
        {
            await _tagRepository.UpdateDiy.Set(r => r.ViewHits + 1)
                  .Where(r => r.Id == tagId)
                  .ExecuteAffrowsAsync();
        }
    }
}
