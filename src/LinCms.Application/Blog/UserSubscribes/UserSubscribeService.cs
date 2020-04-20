using System.Collections.Generic;
using System.Threading.Tasks;
using FreeSql;
using LinCms.Application.Contracts.Blog.UserSubscribes;
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.Blog.UserSubscribes
{
    public class UserSubscribeService : Contracts.Blog.UserSubscribes.IUserLikeService
    {
        private readonly BaseRepository<UserSubscribe> _userSubscribeRepository;
        public UserSubscribeService(BaseRepository<UserSubscribe> userSubscribeRepository)
        {
            _userSubscribeRepository = userSubscribeRepository;
        }

        public async Task<List<long>> GetSubscribeUserIdAsync(long userId)
        {
            List<long> subscribeUserIds = await _userSubscribeRepository
                .Select.Where(r => r.CreateUserId == userId)
                .ToListAsync(r => r.SubscribeUserId);
            return subscribeUserIds;
        }
    }
}
