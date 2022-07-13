using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DotNetCore.Security;
using LinCms.Cms.Users;
using LinCms.Data.Enums;
using LinCms.Dependency;
using LinCms.Entities;
using LinCms.Exceptions;
using LinCms.IRepositories;
using LinCms.Security;
using Microsoft.Extensions.Logging;

namespace LinCms.Cms.Account
{
    [DisableConventionalRegistrationAttribute]
    public class JwtTokenService : ITokenService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserIdentityService _userIdentityService;
        private readonly ILogger<JwtTokenService> _logger;
        private readonly IJwtService _jsonWebTokenService;

        public JwtTokenService(IUserRepository userRepository, ILogger<JwtTokenService> logger, IUserIdentityService userIdentityService, IJwtService jsonWebTokenService)
        {
            _userRepository = userRepository;
            _logger = logger;
            _userIdentityService = userIdentityService;
            _jsonWebTokenService = jsonWebTokenService;
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

            bool valid = await _userIdentityService.VerifyUserPasswordAsync(user.Id, loginInputDto.Password, user.Salt);

            if (!valid)
            {
                throw new LinCmsException("请输入正确密码", ErrorCode.ParameterError);
            }

            _logger.LogInformation($"用户{loginInputDto.Username},登录成功");

            Tokens tokens = await CreateTokenAsync(user);
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

            Tokens tokens = await CreateTokenAsync(user);
            _logger.LogInformation($"用户{user.Username},JwtRefreshToken 刷新-登录成功");

            return tokens;
        }

        private async Task<Tokens> CreateTokenAsync(LinUser user)
        {
            List<Claim> claims = new()
            {
                new Claim (ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim (ClaimTypes.Email, user.Email?? ""),
                new Claim (ClaimTypes.GivenName, user.Nickname?? ""),
                new Claim (ClaimTypes.Name, user.Username?? ""),
            };
            user.LinGroups?.ForEach(r =>
            {
                claims.Add(new Claim(ClaimTypes.Role, r.Name));
                claims.Add(new Claim(LinCmsClaimTypes.Groups, r.Id.ToString()));
            });

            string token = _jsonWebTokenService.Encode(claims);

            user.AddRefreshToken();
            await _userRepository.UpdateAsync(user);

            return new Tokens(token, user.RefreshToken);
        }
    }
}
