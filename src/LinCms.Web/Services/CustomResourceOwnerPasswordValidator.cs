using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Test;
using IdentityServer4.Validation;
using LinCms.Zero.Data.Enums;
using LinCms.Zero.Domain;
using LinCms.Zero.Exceptions;
using Microsoft.AspNetCore.Authentication;

namespace LinCms.Web.Services
{
    /// <summary>
    /// 自定义 Resource owner password 验证器
    /// </summary>
    public class CustomResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        /// <summary>
        /// 数据库或其他介质获取我们用户数据的对象
        /// </summary>
        private readonly IFreeSql _fsql;
        private readonly ISystemClock _clock;

        public CustomResourceOwnerPasswordValidator(ISystemClock clock, IFreeSql fsql)
        {
            _clock = clock;
            this._fsql = fsql;
        }

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var users = _fsql.Select<LinUser>().Where(r => r.Nickname == context.UserName).ToList();

            if (users.Count == 0)
            {
                throw new LinCmsException("用户不存在", ErrorCode.NotFound);
            }

            var user = users.FirstOrDefault(r => r.Password == context.Password);
            if (user == null)
            {
                throw new LinCmsException("密码错误，请输入正确密码!", ErrorCode.ParameterError);
            }

            //此处使用context.UserName, context.Password 用户名和密码来与数据库的数据做校验
            //var user = _users.FindByUsername(context.UserName);

            //验证通过返回结果 
            //subjectId 为用户唯一标识 一般为用户id
            //authenticationMethod 描述自定义授权类型的认证方法 
            //authTime 授权时间
            //claims 需要返回的用户身份信息单元 此处应该根据我们从数据库读取到的用户信息 添加Claims 如果是从数据库中读取角色信息，那么我们应该在此处添加
            context.Result = new GrantValidationResult(
                user.Id.ToString() ?? throw new ArgumentException("Subject ID not set", nameof(user.Id)),
                OidcConstants.AuthenticationMethods.Password, _clock.UtcNow.UtcDateTime,
                new List<Claim>()
                {
                    new Claim(   JwtClaimTypes.Id,user.Id.ToString()),
                    new Claim(   JwtClaimTypes.Email,user.Email),
                    new Claim(   JwtClaimTypes.Name,user.Nickname),
                    new Claim(   "IsActive",user.IsActive().ToString()),
                    new Claim(   "IsAdmin",user.IsAdmin().ToString()),
                    new Claim(   JwtClaimTypes.Role,user.GroupId.ToString())
                });
            return Task.CompletedTask;
        }
    }
}
