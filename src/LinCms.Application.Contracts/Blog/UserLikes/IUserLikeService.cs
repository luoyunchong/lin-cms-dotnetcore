using System.Threading.Tasks;
using LinCms.Application.Contracts.Blog.UserLikes.Dtos;

namespace LinCms.Application.Blog.UserSubscribes
{
    public interface IUserLikeService
    {
        Task<string> CreateOrCancelAsync(CreateUpdateUserLikeDto createUpdateUserLike);
    }
}