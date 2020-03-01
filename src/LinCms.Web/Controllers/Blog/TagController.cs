using System;
using System.Threading.Tasks;
using LinCms.Application.Blog.Tags;
using LinCms.Application.Contracts.Blog.Tags;
using LinCms.Core.Aop;
using LinCms.Core.Data;
using LinCms.Core.Entities.Blog;
using LinCms.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.Blog
{
    [Route("v1/tag")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly AuditBaseRepository<Tag> _tagRepository;
        private readonly ITagService _tagService;
        public TagController(AuditBaseRepository<Tag> tagRepository, ITagService tagService)
        {
            _tagRepository = tagRepository;
            _tagService = tagService;
        }

        [HttpDelete("{id}")]
        [LinCmsAuthorize("删除标签", "标签管理")]
        public UnifyResponseDto DeleteTag(Guid id)
        {
            _tagRepository.Delete(new Tag { Id = id });
            return UnifyResponseDto.Success();
        }

        [HttpGet]
        [LinCmsAuthorize("所有标签", "标签管理")]
        public PagedResultDto<TagListDto> GetAll([FromQuery]TagSearchDto searchDto)
        {
            return _tagService.Get(searchDto);
        }

        [HttpGet("public")]
        public PagedResultDto<TagListDto> Get([FromQuery]TagSearchDto searchDto)
        {
            searchDto.Status = true;
            return _tagService.Get(searchDto);
        }

        [HttpGet("{id}")]
        public Task<TagListDto> GetAsync(Guid id)
        {
            return _tagService.GetAsync(id);
        }

        [HttpPost]
        [LinCmsAuthorize("新增标签", "标签管理")]
        public async Task<UnifyResponseDto> CreateAsync([FromBody] CreateUpdateTagDto createTag)
        {
            await _tagService.CreateAsync(createTag);
            return UnifyResponseDto.Success("新建标签成功");
        }

        [LinCmsAuthorize("编辑标签", "标签管理")]
        [HttpPut("{id}")]
        public async Task<UnifyResponseDto> UpdateAsync(Guid id, [FromBody] CreateUpdateTagDto updateTag)
        {
            await _tagService.UpdateAsync(id, updateTag);
            return UnifyResponseDto.Success("更新标签成功");
        }

        /// <summary>
        /// 标签-校正标签对应文章数量
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        [LinCmsAuthorize("校正文章数量", "标签管理")]
        [HttpPut("correct/{tagId}")]
        public async Task<UnifyResponseDto> CorrectedTagCountAsync(Guid tagId)
        {
            await _tagService.CorrectedTagCountAsync(tagId);
            return UnifyResponseDto.Success();
        }
    }
}