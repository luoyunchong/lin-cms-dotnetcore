namespace LinCms.Cms.Account;

public class AccountContracts
{
    /// <summary>
    /// 注册返回的uuid值
    /// </summary>
    public static string SendEmailCode_EmailCode = "AccountService.SendEmailCode.EmailCode.{0}";
    /// <summary>
    /// 注册时缓存的邮件验证码
    /// </summary>
    public static string SendEmailCode_VerificationCode = "AccountService.SendEmailCode.VerificationCode.{0}";
    
    /// <summary>
    /// 重置密码时邮件验证码
    /// </summary>
    public static string SendPasswordResetCode_VerificationCode = "AccountService.SendPasswordResetCode.VerificationCode.{0}";

    public static string SendPasswordResetCode_VerificationCode_Count = "AccountService.SendPasswordResetCode.VerificationCode.{0}.Count";

}