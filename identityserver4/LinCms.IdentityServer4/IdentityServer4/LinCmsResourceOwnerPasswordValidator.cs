using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using LinCms.Application.Cms.Users;
using LinCms.Application.Contracts.Cms.Users;
using LinCms.Core.Entities;
using LinCms.Core.IRepositories;

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

            bool valid = _userIdentityService.VerifyUsernamePassword(user.Id, context.UserName, context.Password);

            if (!valid)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "请输入正确密码!");
                return;
            }

            await _userRepository.UpdateLastLoginTimeAsync(user.Id);

            //subjectId 为用户唯一标识 一般为用户id
            //authenticationMethod 描述自定义授权类型的认证方法 
            //authTime 授权时间
            //claims 需要返回的用户身份信息单元
            context.Result = new GrantValidationResult(user.Id.ToString(), OidcConstants.AuthenticationMethods.Password);
        }
    }
}
