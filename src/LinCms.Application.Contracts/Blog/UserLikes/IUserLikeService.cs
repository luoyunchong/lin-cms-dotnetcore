using System.Threading.Tasks;

namespace LinCms.Blog.UserLikes
{
    public interface IUserLikeService
    {
        Task<bool> CreateOrCancelAsync(CreateUpdateUserLikeDto createUpdateUserLike);
    }
}