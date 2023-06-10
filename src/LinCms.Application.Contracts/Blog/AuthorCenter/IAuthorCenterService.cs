using System.Threading.Tasks;

namespace LinCms.Blog.AuthorCenter;

/// <summary>
///  创建者中心
/// </summary>
public interface IAuthorCenterService : IApplicationService
{
    /// <summary>
    /// 获取文章统计
    /// </summary>
    /// <returns></returns>
    Task<ArticleCardDto> GetArtcileCardAsync();
}