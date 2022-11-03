using System;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Email;
using IGeekFan.FreeKit.Extras.FreeSql;
using IGeekFan.FreeKit.Extras.Security;
using LinCms.Cms.Users;
using LinCms.Data.Enums;
using LinCms.Data.Options;
using LinCms.Domain.Captcha;
using LinCms.Entities;
using LinCms.Exceptions;
using Microsoft.Extensions.Options;
using MimeKit;

namespace LinCms.Cms.Account;

public class AccountService : ApplicationService, IAccountService
{
    private readonly IAuditBaseRepository<LinUser, long> _userRepository;
    private readonly IEmailSender _emailSender;
    private readonly MailKitOptions _mailKitOptions;
    private readonly IUserIdentityService _userIdentityService;
    private readonly SiteOption _siteOption;
    private readonly ICaptchaManager _captchaManager;
    private readonly LoginCaptchaOption _loginCaptchaOption;
    public AccountService(
        IAuditBaseRepository<LinUser, long> userRepository,
        IEmailSender emailSender,
        IOptions<MailKitOptions> options,
        IUserIdentityService userIdentityService,
        IOptionsMonitor<SiteOption> siteOption,
        ICaptchaManager captchaManager,
        IOptionsMonitor<LoginCaptchaOption> loginCaptchaOption)
    {
        _userRepository = userRepository;
        _emailSender = emailSender;
        _mailKitOptions = options.Value;
        _userIdentityService = userIdentityService;
        _siteOption = siteOption.CurrentValue;
        _captchaManager = captchaManager;
        _loginCaptchaOption = loginCaptchaOption.CurrentValue;
    }

    /// <summary>
    /// 生成无状态的登录验证码
    /// </summary>
    /// <returns>验证码</returns>
    public LoginCaptchaDto GenerateCaptcha()
    {
        if (_loginCaptchaOption.Enabled == false) return new LoginCaptchaDto();

        string captcha = _captchaManager.GetRandomString(CaptchaManager.RandomStrNum);
        string base64String = _captchaManager.GetRandomCaptchaBase64(captcha);
        string tag = _captchaManager.GetTag(captcha, _loginCaptchaOption.Salt);
        return new LoginCaptchaDto(tag, "data:image/png;base64," + base64String);
    }

    /// <summary>
    /// 校验登录验证码
    /// </summary>
    /// <param name="captcha"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    public bool VerifyCaptcha(String captcha, String tag)
    {
        var captchaBo = _captchaManager.DecodeTag(tag, _loginCaptchaOption.Salt);
        long t = _captchaManager.GetTimeStamp();
        return string.Compare(captchaBo.Captcha, captcha, StringComparison.OrdinalIgnoreCase) == 0 && t > captchaBo.Expired;
    }


    #region 以链接的方式激活，暂未使用
    /// <summary>
    /// 以链接的方式激活，暂未使用
    /// </summary>
    /// <param name="sendEmailCodeInput"></param>
    /// <returns></returns>
    /// <exception cref="LinCmsException"></exception>
    public async Task<string> SendChangeEmailAsync(SendEmailCodeInput sendEmailCodeInput)
    {
        var isRepeatEmail = await _userRepository.Select.AnyAsync(r => r.Email == sendEmailCodeInput.Email.Trim());
        if (isRepeatEmail)
        {
            throw new LinCmsException("该邮箱重复，请重新输入", ErrorCode.RepeatField);
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_mailKitOptions.UserName, _mailKitOptions.UserName));
        message.To.Add(new MailboxAddress(CurrentUser.FindName(), sendEmailCodeInput.Email));
        message.Subject = $"vvlog-请点击这里激活您的账号";

        string uuid = Guid.NewGuid().ToString();
        await RedisHelper.SetAsync("SendChangeEmail." + sendEmailCodeInput.Email, uuid, 30 * 60);

        message.Body = new TextPart("html")
        {
            Text = $@"{CurrentUser.FindName()},您好!</br>
感谢您在 vvlog的注册，请点击这里激活您的账号：</br>
<a href='{_siteOption.VVLogDomain}/accounts/confirm-email/{uuid}' target='_blank'></a>
祝您使用愉快，使用过程中您有任何问题请及时联系我们。</br>"
        };

        await _emailSender.SendAsync(message);
        return "";
    } 
    #endregion


    public async Task<string> SendEmailCodeAsync(RegisterEmailCodeInput registerDto)
    {
        var isRepeatEmail = await _userRepository.Select.AnyAsync(r => r.Email == registerDto.Email.Trim());
        if (isRepeatEmail)
        {
            throw new LinCmsException("该邮箱已注册，请更换", ErrorCode.RepeatField);
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_mailKitOptions.UserName, _mailKitOptions.UserName));
        message.To.Add(new MailboxAddress(registerDto.Nickname, registerDto.Email));
        message.Subject = $"vvlog-你的验证码是";

        string uuid = Guid.NewGuid().ToString();
        await RedisHelper.SetAsync("SendEmailCode." + registerDto.Email, uuid, 30 * 60);

        int verificationCode = new Random().Next(100000, 999999);

        message.Body = new TextPart("html")
        {
            Text = $@"{registerDto.Nickname},您好!</br>你此次验证码如下，请在 30 分钟内输入验证码进行下一步操作。</br>如非你本人操作，请忽略此邮件。</br>{verificationCode}"
        };

        await _emailSender.SendAsync(message);
        await RedisHelper.SetAsync("SendEmailCode.VerificationCode." + registerDto.Email, verificationCode, 30 * 60);
        return uuid;
    }

    public async Task<string> SendPasswordResetCodeAsync(SendEmailCodeInput sendEmailCode)
    {
        var user = await GetUserByChecking(sendEmailCode.Email);

        if (user.IsEmailConfirmed == false)
        {
            throw new LinCmsException("邮件未激活,无法通过此邮件找回密码!");
        }

        user.SetNewPasswordResetCode();

        int verificationCode = new Random().Next(100000, 999999);

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_mailKitOptions.UserName, _mailKitOptions.UserName));
        message.To.Add(new MailboxAddress(user.Username, user.Email));
        message.Subject = $"vvlog-你此次重置密码的验证码是:{verificationCode}";

        message.Body = new TextPart("html")
        {
            Text = $@"{user.Nickname},您好!</br>你此次重置密码的验证码如下，请在 30 分钟内输入验证码进行下一步操作。</br>如非你本人操作，请忽略此邮件。</br>{verificationCode}"
        };

        await _userRepository.UpdateAsync(user);

        await _emailSender.SendAsync(message);

        await RedisHelper.SetAsync(user.Email, verificationCode, 30 * 60);

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

    public async Task ResetPasswordAsync(ResetEmailPasswordDto resetPassword)
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
        await _userIdentityService.ChangePasswordAsync(user.Id, resetPassword.Password, user.Salt);
    }
}