using LinCms.Cms.Users;
using LinCms.Email;
using LinCms.Entities;
using LinCms.Exceptions;
using LinCms.IRepositories;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinCms.Cms.Account
{
    public class AccountService : IAccountService
    {
        private readonly IAuditBaseRepository<LinUser, long> _userRepository;
        private readonly IEmailSender _emailSender;
        private readonly MailKitOptions _mailKitOptions;
        private readonly IUserIdentityService _userIdentityService;
        public AccountService(IAuditBaseRepository<LinUser, long> userRepository, IEmailSender emailSender,
            IOptions<MailKitOptions> options, IUserIdentityService userIdentityService)
        {
            _userRepository = userRepository;
            _emailSender = emailSender;
            _mailKitOptions = options.Value;
            _userIdentityService = userIdentityService;
        }

        public async Task<string> SendPasswordResetCode(SendEmailCodeInput sendEmailCode)
        {
            var user = await GetUserByChecking(sendEmailCode.Email);

            if (user.IsEmailConfirmed == true)
            {
                throw new LinCmsException("邮件未激活!");
            }

            user.SetNewPasswordResetCode();

            int rand6Value = new Random().Next(100000, 999999);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_mailKitOptions.UserName, _mailKitOptions.UserName));
            message.To.Add(new MailboxAddress(user.Username, user.Email));
            message.Subject = $"你此次重置密码的验证码是:{rand6Value}";

            message.Body = new TextPart("plain")
            {
                Text = $@"{user.Nickname},您好!</br>你此次重置密码的验证码如下，请在 30 分钟内输入验证码进行下一步操作。</br>如非你本人操作，请忽略此邮件。</br>{rand6Value}"
            };

            await _userRepository.UpdateAsync(user);

            await _emailSender.SendAsync(message);

            await RedisHelper.SetAsync(user.Email, rand6Value, 30 * 60);

            return user.PasswordResetCode;
        }

        private async Task<LinUser> GetUserByChecking(string inputEmailAddress)
        {
            var user = await _userRepository.Select.Where(r => r.Email == inputEmailAddress).FirstAsync();
            if (user == null)
            {
                throw new LinCmsException("InvalidEmailAddress");
            }

            return user;
        }

        public async Task ResetPassword(ResetEmailPasswordDto resetPassword)
        {
            var user = await _userRepository.Select.Where(r => r.Email == resetPassword.Email).FirstAsync();

            string resetCode = await RedisHelper.GetAsync(user.Email);

            if (user == null || resetPassword.ResetCode != resetCode)
            {
                throw new LinCmsException("InvalidEmailConfirmationCode");
            }

            user.PasswordResetCode = null;
            await _userRepository.UpdateAsync(user);

            await _userIdentityService.ChangePasswordAsync(user.Id, resetPassword.Password);

        }
    }
}
