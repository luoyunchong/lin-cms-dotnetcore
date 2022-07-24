using System.Threading.Tasks;

namespace LinCms.Cms.Account
{
    public interface IAccountService
    {
        /// <summary>
        /// 发送邮件：重置密码的验证码
        /// </summary>
        /// <param name="sendEmailCode"></param>
        /// <returns></returns>
        Task<string> SendPasswordResetCodeAsync(SendEmailCodeInput sendEmailCode);

        Task SendEmailCodeAsync(RegisterDto registerDto);

        Task ResetPasswordAsync(ResetEmailPasswordDto resetPassword);
    }
}
