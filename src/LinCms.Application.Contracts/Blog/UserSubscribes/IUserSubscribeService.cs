using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinCms.Application.Contracts.Blog.UserSubscribes
{
    public interface IUserLikeService
    {
        /// <summary>
        /// 得到某用户的关注的用户Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<long>> GetSubscribeUserIdAsync(long userId);
    }
}
