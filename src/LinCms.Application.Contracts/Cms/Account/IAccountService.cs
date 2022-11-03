using System;
using System.Threading.Tasks;
using LinCms.Cms.Users;

namespace LinCms.Cms.Account;

public interface IAccountService
{

    /// <summary>
    /// 生成无状态的登录验证码
    /// </summary>
    /// <returns></returns>
    LoginCaptchaDto GenerateCaptcha();

    /// <summary>
    /// 校验登录验证码
    /// </summary>
    /// <param name="captcha"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    bool VerifyCaptcha(String captcha, String tag);

    /// <summary>
    /// 注册前先发送邮件才能正常注册
    /// </summary>
    /// <param name="registerDto"></param>
    /// <returns></returns>
    Task<string> SendEmailCodeAsync(RegisterEmailCodeInput registerDto);

    /// <summary>
    /// 发送邮件：重置密码的验证码
    /// </summary>
    /// <param name="sendEmailCode"></param>
    /// <returns></returns>
    Task<string> SendPasswordResetCodeAsync(SendEmailCodeInput sendEmailCode);

    /// <summary>
    /// 重置密码：根据邮件验证码
    /// </summary>
    /// <param name="resetPassword"></param>
    /// <returns></returns>

    Task ResetPasswordAsync(ResetEmailPasswordDto resetPassword);
}