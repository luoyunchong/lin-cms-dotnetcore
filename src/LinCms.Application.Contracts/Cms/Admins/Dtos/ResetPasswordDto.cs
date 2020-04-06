using System.ComponentModel.DataAnnotations;

namespace LinCms.Application.Contracts.Cms.Admins.Dtos
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "新密码不可为空")]
        [Compare("ConfirmPassword", ErrorMessage = "两次输入的密码不一致，请输入相同的密码")]
        [RegularExpression("^[A-Za-z0-9_*&$#@]{6,22}$", ErrorMessage = "密码长度必须在6~22位之间，包含字符、数字和 _")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "请确认密码")]
        public string ConfirmPassword { get; set; }
    }
}
