using System;
using System.Threading.Tasks;
using LinCms.Aop.Attributes;
using LinCms.Aop.Filter;
using LinCms.Blog.Tags;
using LinCms.Data;
using LinCms.Entities.Blog;
using LinCms.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Controllers.Blog
{
    [ApiExplorerSettings(GroupName = "blog")]
    [Area("blog")]
    [Route("api/blog/tags")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly IAuditBaseRepository<Tag> _tagRepository;
        private readonly ITagService _tagService;
        public TagController(IAuditBaseRepository<Tag> tagRepository, ITagService tagService)
        {
            _tagRepository = tagRepository;
            _tagService = tagService;
        }

        [HttpDelete("{id}")]
        [LinCmsAuthorize("删除标签", "标签管理")]
        public async Task<UnifyResponseDto> DeleteAsync(Guid id)
        {
            await _tagRepository.DeleteAsync(new Tag { Id = id });
            return UnifyResponseDto.Success();
        }

        [HttpGet]
        [LinCmsAuthorize("所有标签", "标签管理")]
        public async Task<PagedResultDto<TagListDto>> GetAllAsync([FromQuery] TagSearchDto searchDto)
        {
            return await _tagService.GetListAsync(searchDto);
        }

        [Cacheable]
        [HttpGet("public")]
        public virtual async Task<PagedResultDto<TagListDto>> GetListAsync([FromQuery] TagSearchDto searchDto)
        {
            searchDto.Status = true;
            return await _tagService.GetListAsync(searchDto);
        }

        [HttpGet("{id}")]
        public async Task<TagListDto> GetAsync(Guid id)
        {
            await _tagService.IncreaseTagViewHits(id);
            return await _tagService.GetAsync(id);
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
        /// 标签-校正标签对应随笔数量
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        [LinCmsAuthorize("校正随笔数量", "标签管理")]
        [HttpPut("correct/{tagId}")]
        public async Task<UnifyResponseDto> CorrectedTagCountAsync(Guid tagId)
        {
            await _tagService.CorrectedTagCountAsync(tagId);
            return UnifyResponseDto.Success();
        }
    }
}