using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinCms.Cms.Account
{
    public interface IAccountService
    {
        Task<string> SendPasswordResetCode(SendEmailCodeInput sendEmailCode);

        Task ResetPassword(ResetEmailPasswordDto resetPassword);
    }
}
