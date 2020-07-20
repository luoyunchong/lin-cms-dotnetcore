using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Blog.UserSubscribes;
using LinCms.Data;
using LinCms.Data.Enums;
using LinCms.Entities.Blog;
using LinCms.Exceptions;
using LinCms.Extensions;
using LinCms.IRepositories;
using LinCms.Security;

namespace LinCms.Blog.Tags
{
    public class TagService : ApplicationService, ITagService
    {
        private readonly IAuditBaseRepository<UserTag> _userTagRepository;
        private readonly IAuditBaseRepository<Tag> _tagRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        private readonly IAuditBaseRepository<TagArticle> _tagArticleRepository;
        private readonly IFileRepository _fileRepository;
        public TagService(IAuditBaseRepository<Tag> tagRepository, IMapper mapper, ICurrentUser currentUser, IAuditBaseRepository<UserTag> userTagRepository, IAuditBaseRepository<TagArticle> tagArticleRepository, IFileRepository fileRepository)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
            _currentUser = currentUser;
            _userTagRepository = userTagRepository;
            _tagArticleRepository = tagArticleRepository;
            _fileRepository = fileRepository;
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
            tagDto.ThumbnailDisplay = _fileRepository.GetFileUrl(tagDto.Thumbnail);
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
                            tagDto.ThumbnailDisplay = _fileRepository.GetFileUrl(tagDto.Thumbnail);
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
            List<Guid> userTagIds = _userTagRepository.Select
                .Where(u => u.CreateUserId == _currentUser.Id)
                .ToList(r => r.TagId);

            List<TagListDto> tagListDtos = _userTagRepository.Select.Include(r => r.Tag)
                .Where(r => r.CreateUserId == userSubscribeDto.UserId)
                .OrderByDescending(r => r.CreateTime)
                .ToPagerList(userSubscribeDto, out long count)
                .Select(r =>
                {
                    TagListDto tagDto = _mapper.Map<TagListDto>(r.Tag);
                    if (tagDto != null)
                    {
                        tagDto.ThumbnailDisplay = _fileRepository.GetFileUrl(tagDto.Thumbnail);
                        tagDto.IsSubscribe = userTagIds.Any(tagId => tagId == tagDto.Id);
                    }
                    else
                    {
                        return new TagListDto()
                        {
                            Id = r.TagId,
                            TagName = "该标签已被拉黑",
                            IsSubscribe = userTagIds.Any(tagId => tagId == r.TagId)
                        };
                    }
                    return tagDto;
                }).ToList();

            return new PagedResultDto<TagListDto>(tagListDtos, count);
        }

        public async Task UpdateArticleCountAsync(Guid? id, int inCreaseCount)
        {
            if (id == null)
            {
                return;
            }

            //防止数量一直减，减到小于0
            if (inCreaseCount < 0)
            {
                Tag tag = await _tagRepository.Select.Where(r => r.Id == id).ToOneAsync();
                if (tag.ArticleCount < -inCreaseCount)
                {
                    return;
                }
            }

            await _tagRepository.UpdateDiy.Set(r => r.ArticleCount + inCreaseCount)
                 .Where(r => r.Id == id)
                 .ExecuteAffrowsAsync();
        }


        public async Task UpdateSubscribersCountAsync(Guid? id, int inCreaseCount)
        {
            if (id == null)
            {
                return;
            }

            Tag tag = await _tagRepository.Select.Where(r => r.Id == id).ToOneAsync();
            if (tag == null)
            {
                throw new LinCmsException("标签不存在", ErrorCode.NotFound);
            }

            tag.UpdateSubscribersCount(inCreaseCount);
            await _tagRepository.UpdateAsync(tag);
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
