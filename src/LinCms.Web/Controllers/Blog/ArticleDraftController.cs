using System;
using System.Threading.Tasks;

using LinCms.Blog.ArticleDrafts;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Controllers.Blog;

/// <summary>
/// 随笔草稿箱，自动保存随笔
/// </summary>
[Area("blog")]
[ApiExplorerSettings(GroupName = "blog")]
[Route("api/blog/articles/draft")]
[ApiController]
[Authorize]
public class ArticleDraftController(IArticleDraftService articleDraftService) : ControllerBase
{
    [HttpPut("{id}")]
    public Task UpdateAsync(Guid id, [FromBody] UpdateArticleDraftDto updateArticleDto)
    {
        return articleDraftService.UpdateAsync(id, updateArticleDto);
    }

    /// <summary>
    /// 用户的随笔草稿详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public Task<ArticleDraftDto> GetAsync(Guid id)
    {
        return articleDraftService.GetAsync(id);
    }
}