using System.Threading.Tasks;

namespace LinCms.Blog.Collections;

/// <summary>
/// 文章收藏服务
/// </summary>
public interface IArticleCollectionService:IApplicationService
{
    /// <summary>
    /// 收藏或取消收藏文章
    /// </summary>
    /// <param name="crDto"></param>
    /// <returns></returns>
    Task<bool> CreateOrCancelAsync(CreateCancelArticleCollectionDto crDto);
}