using IGeekFan.FreeKit.Extras;
using LinCms.Aop.Filter;
using LinCms.Blog.Tags;
using LinCms.Data;
using LinCms.Entities.Blog;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dto;
using IGeekFan.FreeKit.Extras.FreeSql;

namespace LinCms.Controllers.Blog;

/// <summary>
/// 标签
/// </summary>
[ApiExplorerSettings(GroupName = "blog")]
[Area("blog")]
[Route("api/blog/tags")]
[ApiController]
public class TagController(IAuditBaseRepository<Tag> tagRepository, ITagService tagService)
    : ControllerBase
{
    [HttpDelete("{id}")]
    [LinCmsAuthorize("删除标签", "标签管理")]
    public async Task<UnifyResponseDto> DeleteAsync(Guid id)
    {
        await tagRepository.DeleteAsync(new Tag { Id = id });
        return UnifyResponseDto.Success();
    }

    [HttpGet]
    [LinCmsAuthorize("所有标签", "标签管理")]
    public async Task<PagedResultDto<TagListDto>> GetAllAsync([FromQuery] TagSearchDto searchDto)
    {
        return await tagService.GetListAsync(searchDto);
    }

    [HttpGet("public")]
    public virtual async Task<PagedResultDto<TagListDto>> GetListAsync([FromQuery] TagSearchDto searchDto)
    {
        searchDto.Status = true;
        return await tagService.GetListAsync(searchDto);
    }

    [HttpGet("{id}")]
    public async Task<TagListDto> GetAsync(Guid id)
    {
        await tagService.IncreaseTagViewHits(id);
        return await tagService.GetAsync(id);
    }

    [HttpPost]
    [LinCmsAuthorize("新增标签", "标签管理")]
    public async Task<UnifyResponseDto> CreateAsync([FromBody] CreateUpdateTagDto createTag)
    {
        await tagService.CreateAsync(createTag);
        return UnifyResponseDto.Success("新建标签成功");
    }

    [LinCmsAuthorize("编辑标签", "标签管理")]
    [HttpPut("{id}")]
    public async Task<UnifyResponseDto> UpdateAsync(Guid id, [FromBody] CreateUpdateTagDto updateTag)
    {
        await tagService.UpdateAsync(id, updateTag);
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
        await tagService.CorrectedTagCountAsync(tagId);
        return UnifyResponseDto.Success();
    }
}