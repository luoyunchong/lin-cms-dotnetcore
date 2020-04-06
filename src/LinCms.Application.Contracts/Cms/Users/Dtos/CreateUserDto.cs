using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace LinCms.Application.Contracts.Cms.Users.Dtos
{
    public class CreateUserDto : IValidatableObject
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [StringLength(10, MinimumLength = 2, ErrorMessage = "用户名长度必须在2~10之间")]
        [Required(ErrorMessage = "用户名不能为空")]
        public string Username { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [StringLength(10, MinimumLength = 2, ErrorMessage = "昵称长度必须在2~10之间")]
        [Required(ErrorMessage = "昵称不可为空")]
        public string Nickname { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "新密码不可为空")]
        [Compare("ConfirmPassword", ErrorMessage = "两次输入的密码不一致，请输 入相同的密码")]
        [RegularExpression("^[A-Za-z0-9_*&$#@]{6,22}$", ErrorMessage = "密码长度必须在6~22位之间，包含字符、数字和 _")]
        public string Password { get; set; }
        /// <summary>
        /// 确认密码
        /// </summary>
        [Required(ErrorMessage = "请确认密码")]
        public string ConfirmPassword { get; set; }
        /// <summary>
        /// 电子邮箱
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        //        [EmailAddress(ErrorMessage = "电子邮箱不符合规范，请输入正确的邮箱")] //为空时也不符合电子邮箱格式 
        public string Email { get; set; }
        /// <summary>
        /// 用户所属的权限组id
        /// </summary>
        [Required(ErrorMessage = "请输入分组id")]
        //[Remote("/cms/admin/group/validateGroup", HttpMethod = "Post",ErrorMessage = "分组不存在")]
        public List<long> GroupIds { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Email.IsNullOrEmpty())
            {

                string address = null;
                try
                {
                    address = new MailAddress(Email).Address;
                }
                catch
                {
                    // ignored
                }

                if (address.IsNullOrEmpty())
                {
                    yield return new ValidationResult("电子邮箱不符合规范，请输入正确的邮箱", new[] { "Email" });
                }

            }

        }
    }
}
