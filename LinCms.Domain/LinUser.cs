using System;

namespace LinCms.Domain
{
    public class LinUser : FullAduitEntity
    {
        public string Nickname { get; set; }
        /// <summary>
        ///  用户默认生成图像，为null、头像url
        /// </summary>
        public string Avatar { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int Admin { get; set; } = 1;
        public int Active { get; set; }
        public int GroupId { get; set; }
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
