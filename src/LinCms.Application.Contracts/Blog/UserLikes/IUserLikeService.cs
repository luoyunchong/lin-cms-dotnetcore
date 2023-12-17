using System.Threading.Tasks;

namespace LinCms.Blog.UserLikes;

/// <summary>
/// 用户点赞
/// </summary>
public interface IUserLikeService
{
    /// <summary>
    /// 点赞/取消点赞随笔、评论 
    /// </summary>
    /// <param name="createUpdateUserLike"></param>
    /// <returns></returns>
    Task<bool> CreateOrCancelAsync(CreateUpdateUserLikeDto createUpdateUserLike);
}