using LinCms.Cms.Users;
using LinCms.Data.Options;
using LinCms.Email;
using LinCms.Entities;
using LinCms.Exceptions;
using LinCms.IRepositories;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace LinCms.Cms.Account
{
    public class AccountService : IAccountService
    {
        private readonly IAuditBaseRepository<LinUser, long> _userRepository;
        private readonly IEmailSender _emailSender;
        private readonly MailKitOptions _mailKitOptions;
        private readonly IUserIdentityService _userIdentityService;
        private readonly SiteOption _siteOption;

        public AccountService(
            IAuditBaseRepository<LinUser, long> userRepository,
            IEmailSender emailSender,
            IOptions<MailKitOptions> options,
            IUserIdentityService userIdentityService,
            IOptions<SiteOption> siteOption)
        {
            _userRepository = userRepository;
            _emailSender = emailSender;
            _mailKitOptions = options.Value;
            _userIdentityService = userIdentityService;
            _siteOption = siteOption.Value;
        }

        public async Task<string> SendComfirmEmail(RegisterDto registerDto)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_mailKitOptions.UserName, _mailKitOptions.UserName));
            message.To.Add(new MailboxAddress(registerDto.Nickname, registerDto.Email));
            message.Subject = $"VVLOG-请点击这里激活您的账号";

            string uuid = Guid.NewGuid().ToString();
            await RedisHelper.SetAsync("SendComfirmEmail-" + registerDto.Email, uuid, 30 * 60);

            message.Body = new TextPart("html")
            {
                Text = $@"{registerDto.Nickname},您好!</br>
感谢您在 vvlog  的注册，请点击这里激活您的账号：</br>
{_siteOption.VVLogDomain}/accounts/confirm-email/{uuid}/
祝您使用愉快，使用过程中您有任何问题请及时联系我们。</br>"
            };

            await _emailSender.SendAsync(message);
            return "";
        }

        public async Task SendEmailCode(RegisterDto registerDto)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_mailKitOptions.UserName, _mailKitOptions.UserName));
            message.To.Add(new MailboxAddress(registerDto.Nickname, registerDto.Email));
            message.Subject = $"VVLOG-你的验证码是";

            string uuid = Guid.NewGuid().ToString();
            await RedisHelper.SetAsync("SendEmailCode-" + registerDto.Email, uuid, 30 * 60);

            int rand6Value = new Random().Next(100000, 999999);

            message.Body = new TextPart("html")
            {
                Text = $@"{registerDto.Nickname},您好!</br>你此次验证码如下，请在 30 分钟内输入验证码进行下一步操作。</br>如非你本人操作，请忽略此邮件。</br>{rand6Value}"
            };


            await _emailSender.SendAsync(message);
        }

        public async Task<string> SendPasswordResetCode(SendEmailCodeInput sendEmailCode)
        {
            var user = await GetUserByChecking(sendEmailCode.Email);

            if (user.IsEmailConfirmed == false)
            {
                throw new LinCmsException("邮件未激活,无法通过此邮件找回密码!");
            }

            user.SetNewPasswordResetCode();

            int rand6Value = new Random().Next(100000, 999999);

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_mailKitOptions.UserName, _mailKitOptions.UserName));
            message.To.Add(new MailboxAddress(user.Username, user.Email));
            message.Subject = $"VVLOG-你此次重置密码的验证码是:{rand6Value}";

            message.Body = new TextPart("html")
            {
                Text = $@"{user.Nickname},您好!</br>你此次重置密码的验证码如下，请在 30 分钟内输入验证码进行下一步操作。</br>如非你本人操作，请忽略此邮件。</br>{rand6Value}"
            };

            await _userRepository.UpdateAsync(user);

            //await _emailSender.SendAsync(message);

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
            string resetCode = await RedisHelper.GetAsync(resetPassword.Email);
            if (resetCode.IsNullOrEmpty())
            {
                throw new LinCmsException("验证码已过期");
            }
            if (resetPassword.ResetCode != resetCode)
            {
                throw new LinCmsException("验证码不正确");//InvalidEmailConfirmationCode
            }

            var user = await _userRepository.Select.Where(r => r.Email == resetPassword.Email).FirstAsync();
            if (user == null || resetPassword.PasswordResetCode != user.PasswordResetCode)
            {
                throw new LinCmsException("该请求无效，请重新获取验证码");
            }

            user.PasswordResetCode = null;
            await _userRepository.UpdateAsync(user);
            await _userIdentityService.ChangePasswordAsync(user.Id, resetPassword.Password);
        }
    }
}
