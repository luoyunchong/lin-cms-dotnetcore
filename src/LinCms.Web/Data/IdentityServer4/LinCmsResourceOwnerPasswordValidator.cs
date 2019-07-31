using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using LinCms.Zero.Common;
using LinCms.Zero.Data.Enums;
using LinCms.Zero.Domain;
using LinCms.Zero.Exceptions;
using LinCms.Zero.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace LinCms.Web.Data.IdentityServer4
{
    /// <summary>
    /// 自定义 Resource owner password 验证器
    /// </summary>
    public class LinCmsResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IFreeSql _fsql;
        private readonly ISystemClock _clock;

        public LinCmsResourceOwnerPasswordValidator(ISystemClock clock, IFreeSql fsql)
        {
            _clock = clock;
            _fsql = fsql;
        }

        /// <summary>
        /// 验证密码是否正确,生成Claims，返回用户身份信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = _fsql.Select<LinUser>().Where(r => r.Nickname == context.UserName).ToOne();

            //验证失败
            if (user == null)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "用户不存在");
                return Task.CompletedTask;
            }

            if (user.Password != LinCmsUtils.Get32Md5(context.Password))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "请输入正确密码!");
                return Task.CompletedTask;
            }

            //subjectId 为用户唯一标识 一般为用户id
            //authenticationMethod 描述自定义授权类型的认证方法 
            //authTime 授权时间
            //claims 需要返回的用户身份信息单元
            context.Result = new GrantValidationResult(
                user.Id.ToString(),
                OidcConstants.AuthenticationMethods.Password,
                _clock.UtcNow.UtcDateTime,
                new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                    new Claim(ClaimTypes.Email,user.Email),
                    new Claim(ClaimTypes.Name,user.Nickname),
                    new Claim(LinCmsClaimTypes.GroupId,user.GroupId.ToString()),
                    new Claim(LinCmsClaimTypes.IsAdmin,user.IsAdmin().ToString()),
                    new Claim(ClaimTypes.Role,user.IsAdmin()?LinGroup.Administrator:user.GroupId.ToString())
                });
            return Task.CompletedTask;
        }
    }
}
