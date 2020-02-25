using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using LinCms.Application.Cms.Users;
using LinCms.Core.Common;
using LinCms.Core.Entities;
using LinCms.Core.Security;
using Microsoft.AspNetCore.Authentication;

namespace LinCms.IdentityServer4.IdentityServer4
{
    /// <summary>
    /// 自定义 Resource owner password 验证器
    /// </summary>
    public class LinCmsResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly ISystemClock _clock;
        private readonly IUserIdentityService _userIdentityService;
        private readonly IUserService _userSevice;

        public LinCmsResourceOwnerPasswordValidator(ISystemClock clock, IUserIdentityService userIdentityService, IUserService userSevice)
        {
            _clock = clock;
            this._userIdentityService = userIdentityService;
            _userSevice = userSevice;
        }

        /// <summary>
        /// 验证密码是否正确,生成Claims，返回用户身份信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            LinUser user = await _userSevice.GetUserAsync(r => r.Username == context.UserName);

            if (user == null)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "用户不存在");
                return;
            }

            bool valid = _userIdentityService.VerifyUsernamePassword(user.Id, context.UserName, context.Password);

            if (!valid)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "请输入正确密码!");
                return;
            }

            //_freeSql.Update<LinUser>().Set(r => new LinUser()
            //{
            //    LastLoginTime = DateTime.Now
            //}).Where(r => r.Id == user.Id).ExecuteAffrows();

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.GivenName, user.Nickname ?? ""),
                new Claim(ClaimTypes.Name, user.Username ?? ""),
                new Claim(LinCmsClaimTypes.IsAdmin, user.IsAdmin().ToString()),
            };

            user.LinGroups.ForEach(r =>
             {
                 claims.Add(new Claim(LinCmsClaimTypes.Groups, r.Id.ToString()));
             });

            //subjectId 为用户唯一标识 一般为用户id
            //authenticationMethod 描述自定义授权类型的认证方法 
            //authTime 授权时间
            //claims 需要返回的用户身份信息单元
            context.Result = new GrantValidationResult(
                user.Id.ToString(),
                OidcConstants.AuthenticationMethods.Password,
                 _clock.UtcNow.UtcDateTime,
                 claims
                );
        }
    }
}
