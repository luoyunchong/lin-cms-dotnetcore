using Autofac;
using AutoMapper;
using IGeekFan.FreeKit.Extras.FreeSql;
using IGeekFan.FreeKit.Extras.Security;
using LinCms.Cms.Account;
using LinCms.Cms.Users;
using LinCms.Data;
using LinCms.Data.Enums;
using LinCms.Domain.Captcha;
using LinCms.Entities;
using LinCms.Exceptions;
using LinCms.Middleware;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Aop.Attributes;

namespace LinCms.Controllers.Cms;

/// <summary>
///  账号
/// </summary>
[ApiExplorerSettings(GroupName = "cms")]
[AllowAnonymous]
[ApiController]
[Route("cms/user")]
public class AccountController : ApiControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IAccountService _accountService;
    private readonly IAuditBaseRepository<BlackRecord> _blackRecordRepository;
    private readonly CaptchaOption _loginCaptchaOption;
    private readonly ICaptchaManager _captchaManager;
    public AccountController(IComponentContext componentContext, IConfiguration configuration, IAccountService accountService, IAuditBaseRepository<BlackRecord> blackRecordRepository, IUserService userService, IOptionsMonitor<CaptchaOption> loginCaptchaOption, ICaptchaManager captchaManager)
    {
        bool isIdentityServer4 = configuration.GetSection("Service:IdentityServer4").Value?.ToBoolean() ?? false;
        _tokenService = componentContext.ResolveNamed<ITokenService>(isIdentityServer4 ? nameof(IdentityServer4Service) : nameof(JwtTokenService));
        _accountService = accountService;
        _blackRecordRepository = blackRecordRepository;
        _loginCaptchaOption = loginCaptchaOption.CurrentValue;
        _captchaManager = captchaManager;
    }


    /// <summary>
    /// 生成无状态的登录验证码
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("captcha")]
    public LoginCaptchaDto Captcha()
    {
        return _accountService.GenerateCaptcha();
    }

    /// <summary>
    /// 验证码
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("captchaimg")]
    public IActionResult CaptchaImg()
    {
        string captcha = _captchaManager.GetRandomString(CaptchaManager.RandomStrNum);
        var bytes = ImgHelper.GetVerifyCode(captcha);
        return File(bytes, "image/png");
    }

    /// <summary>
    /// 登录接口
    /// </summary>
    /// <param name="loginInputDto">用户名/密码：admin/123qwe</param>
    [DisableAuditing]
    [ServiceFilter(typeof(RecaptchaVerifyActionFilter))]
    [HttpPost("login")]
    public Task<UserAccessToken> Login([FromBody] LoginInputDto loginInputDto, [FromHeader] string? tag)
    {
        if (_loginCaptchaOption.Enabled)
        {
            if (loginInputDto.Captcha.IsNullOrWhiteSpace())
            {
                throw new LinCmsException("验证码不可为空");
            }
            if (tag.IsNullOrWhiteSpace())
            {
                throw new LinCmsException("非法请求");
            }
            if (!_accountService.VerifyCaptcha(loginInputDto.Captcha, tag))
            {
                throw new LinCmsException("验证码不正确");
            }
        }
        return _tokenService.LoginAsync(loginInputDto);
    }

    /// <summary>
    /// 刷新用户的token
    /// </summary>
    /// <returns></returns>
    [HttpGet("refresh")]
    [AllowAnonymous]
    public async Task<UserAccessToken> GetRefreshTokenAsync()
    {
        string? refreshToken = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (refreshToken == null)
        {
            throw new LinCmsException("请先登录.", ErrorCode.RefreshTokenError);
        }

        return await _tokenService.GetRefreshTokenAsync(refreshToken);
    }

    /// <summary>
    /// 退出登录时，将AccessToken插入到BlackRecord表
    /// </summary>
    /// <returns></returns>
    [HttpGet("logout")]
    [Logger(template: "{user.UserName}退出登录")]
    [Authorize]
    public async Task<UnifyResponseDto> LogoutAsync([FromServices] ICurrentUser currentUser)
    {
        var username = currentUser.UserName;
        string? Jti = await HttpContext.GetTokenAsync("Bearer", "access_token");
        //string Jti = Request.Headers["Authorization"].ToString().Substring(JwtBearerDefaults.AuthenticationScheme.Length + 1).Trim();
        await _blackRecordRepository.InsertAsync(new BlackRecord { Jti = Jti, UserName = username });
        return UnifyResponseDto.Success("退出登录");
    }



    /// <summary>
    /// 注册前先发送邮件才能正常注册
    /// </summary>
    /// <param name="registerDto"></param>
    /// <returns></returns>
    [HttpPost("account/send_email_code")]
    public async Task<string> SendEmailCodeAsync([FromBody] RegisterEmailCodeInput registerDto)
    {
        return await _accountService.SendEmailCodeAsync(registerDto);
    }

    /// <summary>
    /// 注册
    /// </summary>
    /// <param name="registerDto"></param>
    /// <param name="mapper"></param>
    /// <param name="userSevice"></param>
    [Logger("用户注册")]
    [ServiceFilter(typeof(RecaptchaVerifyActionFilter))]
    [HttpPost("account/register")]
    public async Task<UnifyResponseDto> Register([FromBody] RegisterDto registerDto, [FromServices] IMapper mapper, [FromServices] IUserService userSevice)
    {
        //string uuid = await RedisHelper.GetAsync("SendEmailCode." + registerDto.Email);

        //if (uuid != registerDto.EmailCode)
        //{
        //    return UnifyResponseDto.Error("非法请求");
        //}

        //string verificationCode = await RedisHelper.GetAsync("SendEmailCode.VerificationCode" + registerDto.Email);
        //if (verificationCode != registerDto.VerificationCode)
        //{
        //    return UnifyResponseDto.Error("验证码不正确");
        //}
        //暂时设置直接激活，因前台未同步改造成功
        LinUser user = mapper.Map<LinUser>(registerDto);
        user.IsEmailConfirmed = true;
        await userSevice.CreateAsync(user, new List<long>(), registerDto.Password);
        return UnifyResponseDto.Success("注册成功");
    }

    /// <summary>
    /// 发送邮件：重置密码的验证码
    /// </summary>
    /// <param name="sendEmailCode"></param>
    /// <returns></returns>
    [HttpPost("account/send_password_reset_code")]
    public async Task<string> SendPasswordResetCodeAsync([FromBody] SendEmailCodeInput sendEmailCode)
    {
        return await _accountService.SendPasswordResetCodeAsync(sendEmailCode);
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    /// <param name="resetPassword"></param>
    /// <returns></returns>
    [HttpPost("account/reset_password")]
    public Task ResetPasswordAsync([FromBody] ResetEmailPasswordDto resetPassword)
    {
        return _accountService.ResetPasswordAsync(resetPassword);
    }
}