using System;
using System.Collections.Generic;
using FreeSql;
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.Blog.UserSubscribes
{
    public class UserSubscribeService : IUserSubscribeService
    {
        private readonly BaseRepository<UserSubscribe> _userSubscribeRepository;
        public UserSubscribeService(BaseRepository<UserSubscribe> userSubscribeRepository)
        {
            _userSubscribeRepository = userSubscribeRepository;
        }

        public List<long> GetSubscribeUserId(long userId)
        {
            List<long> subscribeUserIds = _userSubscribeRepository
                .Select.Where(r => r.CreateUserId == userId)
                .ToList(r => r.SubscribeUserId);
            return subscribeUserIds;
        }
    }
}
