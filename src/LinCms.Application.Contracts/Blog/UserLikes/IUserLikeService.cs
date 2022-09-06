using System.Threading.Tasks;

namespace LinCms.Blog.UserLikes;

public interface IUserLikeService
{
    /// <summary>
    /// 点赞/取消点赞随笔、评论 
    /// </summary>
    /// <param name="createUpdateUserLike"></param>
    /// <returns></returns>
    Task<bool> CreateOrCancelAsync(CreateUpdateUserLikeDto createUpdateUserLike);
}