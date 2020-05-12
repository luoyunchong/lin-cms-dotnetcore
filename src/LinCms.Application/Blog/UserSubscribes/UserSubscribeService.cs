using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FreeSql;
using LinCms.Application.Contracts.Blog.UserSubscribes;
using LinCms.Core.Entities.Blog;
using LinCms.Core.IRepositories;

namespace LinCms.Application.Blog.UserSubscribes
{
    public class UserSubscribeService : Contracts.Blog.UserSubscribes.IUserLikeService
    {
        private readonly IAuditBaseRepository<UserSubscribe,Guid> _userSubscribeRepository;
        public UserSubscribeService(IAuditBaseRepository<UserSubscribe,Guid> userSubscribeRepository)
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
