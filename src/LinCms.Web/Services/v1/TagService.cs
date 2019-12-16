using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Web.Models.v1.Tags;
using LinCms.Web.Services.v1.Interfaces;
using LinCms.Zero.Data;
using LinCms.Zero.Domain.Blog;
using LinCms.Zero.Extensions;
using LinCms.Zero.Repositories;
using LinCms.Zero.Security;

namespace LinCms.Web.Services.v1
{
    public class TagService : ITagService
    {
        private readonly AuditBaseRepository<Tag> _tagRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        public TagService(AuditBaseRepository<Tag> tagRepository, IMapper mapper, ICurrentUser currentUser)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public PagedResultDto<TagDto> Get(TagSearchDto searchDto)
        {
            if (searchDto.Sort.IsNullOrEmpty())
            {
                searchDto.Sort = "create_time desc";
            }

            List<TagDto> tags = _tagRepository.Select
                .WhereIf(searchDto.TagIds.IsNotNullOrEmpty(), r => searchDto.TagIds.Contains(r.Id))
                .WhereIf(searchDto.TagName.IsNotNullOrEmpty(), r => r.TagName.Contains(searchDto.TagName))
                .WhereIf(searchDto.Status != null, r => r.Status == searchDto.Status)
                .OrderBy(searchDto.Sort)
                .ToPagerList(searchDto, out long totalCount)
                .Select(r =>
                {
                    TagDto tagDto = _mapper.Map<TagDto>(r);
                    tagDto.ThumbnailDisplay = _currentUser.GetFileUrl(tagDto.Thumbnail);
                    return tagDto;
                }).ToList();

            return new PagedResultDto<TagDto>(tags, totalCount);
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

            _tagRepository.UpdateDiy.Set(r => r.ArticleCount + inCreaseCount).Where(r => r.Id == id)
                .ExecuteAffrows();
        }
    }
}
