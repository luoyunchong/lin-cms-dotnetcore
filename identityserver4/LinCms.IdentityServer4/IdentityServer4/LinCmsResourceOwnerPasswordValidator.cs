using System;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using LinCms.Cms.Users;
using LinCms.Data.Enums;
using LinCms.Entities;
using LinCms.IRepositories;

namespace LinCms.IdentityServer4.IdentityServer4
{
    /// <summary>
    /// 自定义 Resource owner password 验证器
    /// </summary>
    public class LinCmsResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IUserIdentityService _userIdentityService;
        private readonly IUserRepository _userRepository;

        public LinCmsResourceOwnerPasswordValidator(IUserIdentityService userIdentityService, IUserRepository userRepository)
        {
            _userIdentityService = userIdentityService;
            _userRepository = userRepository;
        }

        /// <summary>
        /// 验证密码是否正确,生成Claims，返回用户身份信息
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            LinUser user = await _userRepository.Select.Where(r => r.Username == context.UserName || r.Email == context.UserName).ToOneAsync();

            if (user == null)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "用户不存在");
                return;
            }

            if (user.Active == UserActive.NotActive)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "用户未激活");
                return;
            }

            bool valid = await _userIdentityService.VerifyUserPasswordAsync(user.Id, context.Password, user.Salt);

            if (!valid)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "请输入正确密码!");
                return;
            }

            user.LastLoginTime = DateTime.Now;
            await _userRepository.UpdateAsync(user);

            //subjectId 为用户唯一标识 一般为用户id
            //authenticationMethod 描述自定义授权类型的认证方法 
            //authTime 授权时间
            //claims 需要返回的用户身份信息单元
            context.Result = new GrantValidationResult(user.Id.ToString(), OidcConstants.AuthenticationMethods.Password);
        }
    }
}
