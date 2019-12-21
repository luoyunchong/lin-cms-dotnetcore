using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Web.Models.Cms.Users;

namespace LinCms.Web.Models.v1.UserSubscribes
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
        public long? CreateUserId { get; set; }

        /// <summary>
        /// 关注者
        /// </summary>
        public OpenUserDto Subscribeer { get; set; }

        public bool IsSubscribeed { get; set; }

    }
}
