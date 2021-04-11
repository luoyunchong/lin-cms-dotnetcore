using FreeSql.DataAnnotations;
using LinCms.Data.Enums;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace LinCms.Entities
{
    /// <summary>
    /// 用户
    /// </summary>
    [Table(Name = "lin_user")]
    public class LinUser : FullAduitEntity
    {
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
        public UserActive Active { get; set; } = UserActive.Active;

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
        [Column(StringLength = 100)]
        public string BlogAddress { get; set; }

        /// <summary>
        /// 最后一次登录的时间
        /// </summary>
        public DateTime LastLoginTime { get; set; }

        /// <summary>
        /// JWT 登录，保存生成的随机token值。
        /// </summary>
        [Column(StringLength = 200)]
        public string RefreshToken { get; set; }
        public bool IsEmailConfirmed { get; set; } = false;
        public string PasswordResetCode { get; set; }

        [Navigate(ManyToMany = typeof(LinUserGroup))]
        public virtual ICollection<LinGroup> LinGroups { get; set; }

        [Navigate("UserId")]
        public virtual ICollection<LinUserGroup> LinUserGroups { get; set; }

        [Navigate("CreateUserId")]
        public virtual ICollection<LinUserIdentity> LinUserIdentitys { get; set; }


        public void SetNewPasswordResetCode()
        {
            this.PasswordResetCode = Guid.NewGuid().ToString("N").Truncate(328);
        }

        public bool IsActive()
        {
            return Active == UserActive.Active;
        }

        /// <summary>
        /// 登录后用户状态变化
        /// </summary>
        public void AddRefreshToken()
        {
            string refreshToken = GenerateToken();
            LastLoginTime = DateTime.Now;
            RefreshToken = refreshToken;
        }

        private string GenerateToken(int size = 32)
        {
            var randomNumber = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
