﻿using LinCms.Application.Contracts.Cms.Users;

namespace LinCms.Application.Contracts.Blog.UserSubscribes
{
    public class UserSubscribeDto
    {
        /// <summary>
        /// 被关注的用户Id
        /// </summary>
        public long SubscribeUserId { get; set; }
        /// <summary>
        /// 关注的用户Id
        /// </summary>
        public long CreateUserId { get; set; }

        /// <summary>
        /// 关注者
        /// </summary>
        public OpenUserDto Subscribeer { get; set; }

        public bool IsSubscribeed { get; set; }

    }
}
