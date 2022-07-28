using System.Threading.Tasks;

namespace LinCms.Cms.Account
{
    public interface IAccountService
    {
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
}
