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

public class JwtTokenService(IUserRepository userRepository, ILogger<JwtTokenService> logger,
        IUserIdentityService userIdentityService, ITokenManager tokenManager)
    : ITokenService
{
    /// <summary>
    /// JWT登录
    /// </summary>
    /// <param name="loginInputDto"></param>
    /// <returns></returns>
    public async Task<UserAccessToken> LoginAsync(LoginInputDto loginInputDto)
    {
        logger.LogInformation("JwtLogin");

        LinUser user = await userRepository.GetUserAsync(r => r.Username == loginInputDto.Username || r.Email == loginInputDto.Username);

        if (user == null)
        {
            throw new LinCmsException("用户不存在", ErrorCode.NotFound);
        }

        if (user.Active == UserStatus.NotActive)
        {
            throw new LinCmsException("用户未激活", ErrorCode.NoPermission);
        }

        bool valid = await userIdentityService.VerifyUserPasswordAsync(user.Id, loginInputDto.Password, user.Salt);

        if (!valid)
        {
            throw new LinCmsException("请输入正确密码", ErrorCode.ParameterError);
        }

        logger.LogInformation($"用户{loginInputDto.Username},登录成功");

        UserAccessToken tokens = await tokenManager.CreateTokenAsync(user);
        return tokens;
    }


    public async Task<UserAccessToken> GetRefreshTokenAsync(string refreshToken)
    {
        LinUser user = await userRepository.GetUserAsync(r => r.RefreshToken == refreshToken);

        if (user.IsNull())
        {
            throw new LinCmsException("该refreshToken无效!", ErrorCode.RefreshTokenError);
        }

        if (DateTime.Compare(user.LastLoginTime, DateTime.Now) > new TimeSpan(30, 0, 0, 0).Ticks)
        {
            throw new LinCmsException("请重新登录", ErrorCode.RefreshTokenError);
        }

        UserAccessToken tokens = await tokenManager.CreateTokenAsync(user);
        
        return tokens;
    }

}