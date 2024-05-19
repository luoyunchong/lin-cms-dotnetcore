using System.ComponentModel.DataAnnotations;

namespace LinCms.Cms.Account;

public class ResetEmailPasswordDto
{
    [Required(ErrorMessage = "非法请求")]
    public string Email { get; set; }
    [Required(ErrorMessage = "非法请求")]
    public string PasswordResetCode { get; set; }

    [Required(ErrorMessage = "请输入验证码")]
    public string ResetCode { get; set; }
    [Required(ErrorMessage = "请输入你的新密码")]
    [RegularExpression(AccountContract.PasswordRegex, ErrorMessage = AccountContract.PasswordErrorMessage)]
    public string Password { get; set; }
}

public class AccountContract
{
    public const string PasswordRegex = "^(?![a-zA-Z]+$)(?!\\d+$)(?![^\\da-zA-Z\\s]+$).{6,20}$";
    public const string PasswordErrorMessage = "密码由字母、数字、特殊字符，任意2种组成，长度在6-20个字符之间";
}