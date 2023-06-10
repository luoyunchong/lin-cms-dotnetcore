using System;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dto;
using LinCms.Blog.Collections;
using LinCms.Data;
using LinCms.Entities.Blog;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Controllers.Blog;

/// <summary>
/// 收藏
/// </summary>
[ApiExplorerSettings(GroupName = "blog")]
[Area("blog")]
[Route("api/blog/collection")]
[ApiController]
public class CollectionController : ControllerBase
{
    private readonly ICollectionService _collectionService;
    private readonly IArticleCollectionService _articleCollectionService;

    public CollectionController(ICollectionService collectionService,
        IArticleCollectionService articleCollectionService)
    {
        _collectionService = collectionService;
        _articleCollectionService = articleCollectionService;
    }

    [HttpGet]
    public async Task<PagedResultDto<Collection>> GetListAsync([FromQuery] CollectionSearchDto searchdto)
    {
        return await _collectionService.GetListAsync(searchdto);
    }

    [HttpPost]
    public async Task<UnifyResponseDto> CreateAsync([FromBody] CreateUpdateCollectionDto cCollectionDto)
    {
        await _collectionService.CreateAsync(cCollectionDto);
        return UnifyResponseDto.Success("新增成功");
    }

    [HttpPut("{id}")]
    public async Task<UnifyResponseDto> UpdateAsync(Guid id, [FromBody] CreateUpdateCollectionDto uCollectionDto)
    {
        await _collectionService.UpdateAsync(id, uCollectionDto);
        return UnifyResponseDto.Success("修改成功");
    }

    [HttpDelete("{id}")]
    public async Task<UnifyResponseDto> DeleteAsync(Guid id)
    {
        await _collectionService.DeleteAsync(id);
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
        var ok = await _articleCollectionService.CreateOrCancelAsync(crDto);
        return UnifyResponseDto.Success(ok ? "收藏成功" : "取消收藏成功");
    }
}