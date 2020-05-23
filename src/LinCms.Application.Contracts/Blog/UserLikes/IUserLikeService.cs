using System.Threading.Tasks;
using LinCms.Application.Contracts.Blog.UserLikes.Dtos;

namespace LinCms.Application.Contracts.Blog.UserLikes
{
    public interface IUserLikeService
    {
        Task<bool> CreateOrCancelAsync(CreateUpdateUserLikeDto createUpdateUserLike);
    }
}