using System;
using System.Collections.Generic;
using System.Text;
using FreeSql.DataAnnotations;

namespace LinCms.Zero.Domain.Blog
{
    /// <summary>
    /// 用户关注用户
    /// </summary>
    [Table(Name = "blog_user_follow")]
    public class UserFollow : Entity<Guid>, ICreateAduitEntity
    {
        /// <summary>
        /// 被关注的用户Id
        /// </summary>
        public long FollowUserId { get; set; }
        /// <summary>
        /// 关注的用户Id
        /// </summary>
        public long? CreateUserId { get; set; }
        public DateTime CreateTime { get; set; }

        [Navigate("CreateUserId")]
        public virtual LinUser LinUser { get; set; }
        [Navigate("FollowUserId")]
        public virtual LinUser FollowUser { get; set; }
    }
}
