using System.ComponentModel.DataAnnotations;

namespace LinCms.Cms.Admins;

public class ResetPasswordDto
{
    [Required(ErrorMessage = "新密码不可为空")]
    [Compare("ConfirmPassword", ErrorMessage = "两次输入的密码不一致，请输入相同的密码")]
    [RegularExpression(@"^(?=.*[0-9])(?=.*[a-zA-Z])(?=.*[^a-zA-Z0-9]).{8,24}", ErrorMessage = "密码长度必须在8~24位之间，包含字母、数字、特殊字符")]
    public string NewPassword { get; set; }
    [Required(ErrorMessage = "请确认密码")]
    public string ConfirmPassword { get; set; }
}