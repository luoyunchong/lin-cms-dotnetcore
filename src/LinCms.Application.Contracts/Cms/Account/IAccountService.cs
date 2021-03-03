using System.Threading.Tasks;

namespace LinCms.Cms.Account
{
    public interface IAccountService
    {
        Task<string> SendPasswordResetCode(SendEmailCodeInput sendEmailCode);

        Task SendEmailCode(RegisterDto registerDto);

        Task ResetPassword(ResetEmailPasswordDto resetPassword);
    }
}
