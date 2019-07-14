using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LinCms.Web.Models.Users
{
    public class UserInputDto
    {
        /// <summary>
        /// 昵称
        /// </summary>
        [MinLength(length:2,ErrorMessage = "昵称长度必须在2~10之间")]
        [MaxLength(length:10,ErrorMessage = "昵称长度必须在2~10之间")]
        public string Nickname { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 确认密码
        /// </summary>
        public string ConfirmPassword { get; set; }
        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 用户所属的权限组id
        /// </summary>
        public int GroupId { get; set; }
    }
}
