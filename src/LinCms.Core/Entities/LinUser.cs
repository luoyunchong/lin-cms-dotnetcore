using System;
using System.Collections.Generic;

using FreeSql.DataAnnotations;
using LinCms.Core.Data.Enums;

namespace LinCms.Core.Entities
{
    /// <summary>
    /// 用户信息
    /// </summary>
    [Table(Name = "lin_user")]
    public class LinUser : FullAduitEntity
    {
        public LinUser() { }

        public LinUser(string username)
        {
            this.Username = username;
        }

        /// <summary>
        /// 用户名
        /// </summary>
        [Column(StringLength = 24)]
        public string Username { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [Column(StringLength = 24)]
        public string Nickname { get; set; }

        /// <summary>
        ///  用户默认生成图像，为null、头像url
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        [Column(StringLength = 100)]
        public string Email { get; set; }

        /// <summary>
        /// 当前用户是否为激活状态，非激活状态默认失去用户权限 ; 1 -> 激活 | 2 -> 非激活
        /// </summary>
        public UserActive Active { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [Column(StringLength = 100)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 个人介绍
        /// </summary>
        [Column(StringLength = 100)]
        public string Introduction { get; set; }

        /// <summary>
        ///  个人主页
        /// </summary>
        public string BlogAddress { get; set; }

        /// <summary>
        /// 最后一次登录的时间
        /// </summary>
        public DateTime LastLoginTime { get; set; }


        [Navigate(ManyToMany = typeof(LinUserGroup))]
        public virtual ICollection<LinGroup> LinGroups { get; set; }
        [Navigate("UserId")]
        public virtual ICollection<LinUserGroup> LinUserGroups { get; set; }
        [Navigate("CreateUserId")]
        public virtual ICollection<LinUserIdentity> LinUserIdentitys { get; set; }

        public bool IsActive()
        {
            return Active == UserActive.Active;
        }

    }
}
