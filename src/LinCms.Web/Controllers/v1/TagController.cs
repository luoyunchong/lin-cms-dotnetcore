using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LinCms.Web.Models.v1.Tags;
using LinCms.Zero.Aop;
using LinCms.Zero.Data;
using LinCms.Zero.Domain.Blog;
using LinCms.Zero.Exceptions;
using LinCms.Zero.Extensions;
using LinCms.Zero.Repositories;
using LinCms.Zero.Security;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.v1
{
    [Route("v1/tag")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly AuditBaseRepository<Tag> _tagRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        public TagController(AuditBaseRepository<Tag> tagRepository, IMapper mapper, ICurrentUser currentUser)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        [HttpDelete("{id}")]
        [LinCmsAuthorize("删除标签", "标签管理")]
        public ResultDto DeleteTag(Guid id)
        {
            _tagRepository.Delete(new Tag { Id = id });
            return ResultDto.Success();
        }

        [HttpGet]
        public PagedResultDto<TagDto> Get([FromQuery]TagSearchDto searchDto) 
        {
            List<TagDto> tags = _tagRepository.Select
                .WhereIf(searchDto.TagName.IsNotNullOrEmpty(),r=>r.TagName.Contains(searchDto.TagName))
                .OrderByDescending(r => r.Id)
                .ToPagerList(searchDto,out long totalCount)
                .Select(r =>
                {
                    TagDto tagDto= _mapper.Map<TagDto>(r);
                    tagDto.ThumbnailDisplay= _currentUser.GetFileUrl(tagDto.Thumbnail);
                    return tagDto;
                }).ToList();

            return new PagedResultDto<TagDto>(tags,totalCount);
        }

        [HttpGet("{id}")]
        public TagDto Get(Guid id)
        {
            Tag tag = _tagRepository.Select.Where(a => a.Id == id).ToOne();
            TagDto tagDto= _mapper.Map<TagDto>(tag);
            tagDto.ThumbnailDisplay = _currentUser.GetFileUrl(tagDto.Thumbnail);
            return tagDto;
        }

        [HttpPost]
        [LinCmsAuthorize("新增标签", "标签管理")]
        public ResultDto Post([FromBody] CreateUpdateTagDto createTag)
        {
            bool exist = _tagRepository.Select.Any(r => r.TagName==createTag.TagName);
            if (exist)
            {
                throw new LinCmsException($"标签[{createTag.TagName}]已存在");
            }

            Tag tag = _mapper.Map<Tag>(createTag);
            _tagRepository.Insert(tag);
            return ResultDto.Success("新建标签成功");
        }

        [LinCmsAuthorize("编辑标签", "标签管理")]

        [HttpPut("{id}")]
        public ResultDto Put(Guid id, [FromBody] CreateUpdateTagDto updateTag)
        {
            Tag tag = _tagRepository.Select.Where(r => r.Id == id).ToOne();
            if (tag == null)
            {
                throw new LinCmsException("该数据不存在");
            }


            bool exist = _tagRepository.Select.Any(r => r.TagName == updateTag.TagName && r.Id != id);
            if (exist)
            {
                throw new LinCmsException($"标签[{updateTag.TagName}]已存在");
            }

            _mapper.Map(updateTag, tag);

            _tagRepository.Update(tag);

            return ResultDto.Success("更新标签成功");
        }

    }
}