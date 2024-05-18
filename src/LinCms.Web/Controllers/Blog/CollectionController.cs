using IGeekFan.FreeKit.Extras.Dto;
using LinCms.Blog.Collections;
using LinCms.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LinCms.Controllers.Blog;

/// <summary>
/// 收藏
/// </summary>
[ApiExplorerSettings(GroupName = "blog")]
[Area("blog")]
[Route("api/blog/collection")]
[ApiController]
public class CollectionController(ICollectionService collectionService,
        IArticleCollectionService articleCollectionService)
    : ControllerBase
{
    [HttpGet]
    public async Task<PagedResultDto<CollectionDto>> GetListAsync([FromQuery] CollectionSearchDto searchdto)
    {
        return await collectionService.GetListAsync(searchdto);
    }

    [HttpGet("{id}")]
    public async Task<CollectionDto> GetAsync(Guid id)
    {
        return await collectionService.GetAsync(id);
    }

    [HttpPost]
    public async Task<UnifyResponseDto> CreateAsync([FromBody] CreateUpdateCollectionDto cCollectionDto)
    {
        await collectionService.CreateAsync(cCollectionDto);
        return UnifyResponseDto.Success("新增成功");
    }

    [HttpPut("{id}")]
    public async Task<UnifyResponseDto> UpdateAsync(Guid id, [FromBody] CreateUpdateCollectionDto uCollectionDto)
    {
        await collectionService.UpdateAsync(id, uCollectionDto);
        return UnifyResponseDto.Success("修改成功");
    }

    [HttpDelete("{id}")]
    public async Task<UnifyResponseDto> DeleteAsync(Guid id)
    {
        await collectionService.DeleteAsync(id);
        return UnifyResponseDto.Success("删除成功");
    }

    /// <summary>
    /// 收藏文章、取消收藏文章
    /// </summary>
    /// <param name="crDto"></param>
    /// <returns></returns>
    [HttpPost("article")]
    public async Task<UnifyResponseDto> CreateOrCancelAsync([FromBody] CreateCancelArticleCollectionDto crDto)
    {
        var ok = await articleCollectionService.CreateOrCancelAsync(crDto);
        return UnifyResponseDto.Success(ok ? "收藏成功" : "取消收藏成功");
    }
}