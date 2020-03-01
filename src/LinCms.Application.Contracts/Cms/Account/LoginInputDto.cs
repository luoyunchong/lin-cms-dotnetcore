using System.ComponentModel.DataAnnotations;

namespace LinCms.Application.Contracts.Cms.Account
{
    public class LoginInputDto
    {
        /// <summary>
        /// 登录名:admin
        /// </summary>
        [Required(ErrorMessage = "登录名为必填项")]
        public string Username { get; set; }
        /// <summary>
        /// 密码：123qwe
        /// </summary>
        [Required(ErrorMessage = "密码为必填项")]
        public string Password { get; set; }
    }
}
