using System.Threading.Tasks;
using LinCms.Blog.AuthorCenter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Controllers.Blog;

/// <summary>
/// 随笔草稿箱，自动保存随笔
/// </summary>
[Area("blog")]
[ApiExplorerSettings(GroupName = "blog")]
[Route("api/blog/author_center")]
[ApiController]
[Authorize]
public class AuthorCenterController(IAuthorCenterService authorCenterService) : ControllerBase
{
    [HttpGet("card")]
    public async Task<ArticleCardDto> GetArtcileCardAsync()
    {
        return await authorCenterService.GetArtcileCardAsync();
    }
}