using System;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dependency;
using IGeekFan.FreeKit.Extras.Security;
using LinCms.Cms.Users;
using LinCms.Data.Enums;
using LinCms.Domain;
using LinCms.Entities;
using LinCms.Exceptions;
using LinCms.IRepositories;
using Microsoft.Extensions.Logging;

namespace LinCms.Cms.Account;

[DisableConventionalRegistration]
public class JwtTokenService : ITokenService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserIdentityService _userIdentityService;
    private readonly ILogger<JwtTokenService> _logger;
    private readonly ITokenManager _tokenManager;

    public JwtTokenService(IUserRepository userRepository, ILogger<JwtTokenService> logger, IUserIdentityService userIdentityService,ITokenManager tokenManager)
    {
        _userRepository = userRepository;
        _logger = logger;
        _userIdentityService = userIdentityService;
        _tokenManager = tokenManager;
    }
    /// <summary>
    /// JWT登录
    /// </summary>
    /// <param name="loginInputDto"></param>
    /// <returns></returns>
    public async Task<Tokens> LoginAsync(LoginInputDto loginInputDto)
    {
        _logger.LogInformation("JwtLogin");

        LinUser user = await _userRepository.GetUserAsync(r => r.Username == loginInputDto.Username || r.Email == loginInputDto.Username);

        if (user == null)
        {
            throw new LinCmsException("用户不存在", ErrorCode.NotFound);
        }

        if (user.Active == UserStatus.NotActive)
        {
            throw new LinCmsException("用户未激活", ErrorCode.NoPermission);
        }

        bool valid = await _userIdentityService.VerifyUserPasswordAsync(user.Id, loginInputDto.Password, user.Salt);

        if (!valid)
        {
            throw new LinCmsException("请输入正确密码", ErrorCode.ParameterError);
        }

        _logger.LogInformation($"用户{loginInputDto.Username},登录成功");

        Tokens tokens = await _tokenManager.CreateTokenAsync(user);
        return tokens;
    }


    public async Task<Tokens> GetRefreshTokenAsync(string refreshToken)
    {
        LinUser user = await _userRepository.GetUserAsync(r => r.RefreshToken == refreshToken);

        if (user.IsNull())
        {
            throw new LinCmsException("该refreshToken无效!");
        }

        if (DateTime.Compare(user.LastLoginTime, DateTime.Now) > new TimeSpan(30, 0, 0, 0).Ticks)
        {
            throw new LinCmsException("请重新登录", ErrorCode.RefreshTokenError);
        }

        Tokens tokens = await _tokenManager.CreateTokenAsync(user);
        _logger.LogInformation($"用户{user.Username},JwtRefreshToken 刷新-登录成功");

        return tokens;
    }

}