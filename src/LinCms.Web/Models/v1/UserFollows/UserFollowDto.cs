using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Web.Models.Cms.Users;

namespace LinCms.Web.Models.v1.UserFollows
{
    public class UserFollowDto
    {
        /// <summary>
        /// 被关注的用户Id
        /// </summary>
        public long FollowUserId { get; set; }
        /// <summary>
        /// 关注的用户Id
        /// </summary>
        public long? CreateUserId { get; set; }

        /// <summary>
        /// 关注者
        /// </summary>
        public OpenUserDto Follower { get; set; }

        public bool IsFollowed { get; set; }

    }
}
