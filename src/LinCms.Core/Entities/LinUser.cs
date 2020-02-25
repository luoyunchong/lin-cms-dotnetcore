using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FreeSql.DataAnnotations;
using LinCms.Core.Data.Enums;
using LinCms.Core.Entities.Blog;

namespace LinCms.Core.Entities
{
    /// <summary>
    /// 用户信息
    /// </summary>
    [Table(Name = "lin_user")]
    public class LinUser : FullAduitEntity
    {
        public LinUser() { }

        public LinUser(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }

        /// <summary>
        /// 用户名
        /// </summary>
        [Column(DbType = "varchar(24)")]
        public string Username { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [Column(DbType = "varchar(24)")]
        public string Nickname { get; set; }

        /// <summary>
        ///  用户默认生成图像，为null、头像url
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Column(DbType = "varchar(100)")]
        public string Password { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        [Column(DbType = "varchar(100)")]
        public string Email { get; set; }

        /// <summary>
        /// 是否为超级管理员 ;  1 -> 普通用户 |  2 -> 超级管理员
        /// </summary>
        public int Admin { get; set; } = 1;

        /// <summary>
        /// 当前用户是否为激活状态，非激活状态默认失去用户权限 ; 1 -> 激活 | 2 -> 非激活
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [Column(DbType = "varchar(100)")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 个人介绍
        /// </summary>
        [Column(DbType = "varchar(100)")]
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

        /// <summary>
        /// 是否是管理员
        /// </summary>
        /// <returns></returns>
        public bool IsAdmin()
        {
            return Admin == (int)UserAdmin.Admin;
        }

        public bool IsActive()
        {
            return Active == (int)UserActive.Active;
        }

    }
}
