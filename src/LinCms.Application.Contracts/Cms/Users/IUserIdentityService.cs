using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using LinCms.Data;
using LinCms.Entities;

namespace LinCms.Cms.Users
{
 
    public interface IUserIdentityService
    {
        /// <summary>
        /// 保存GitHub登录信息，生成用户表，用户身份认证表
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        Task<long> SaveGitHubAsync(ClaimsPrincipal principal, string openId);
        /// <summary>
        ///  保存QQ快速登录后的登录信息，生成用户
        /// </summary>
        /// <param name="principal"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        Task<long> SaveQQAsync(ClaimsPrincipal principal, string openId);
        Task<long> SaveGiteeAsync(ClaimsPrincipal principal, string openId);
        /// <summary>
        /// 验证用户密码是否正确
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<bool> VerifyUserPasswordAsync(long userId, string password);
        
        /// <summary>
        /// 根据linUserIdentity生成密码
        /// </summary>
        /// <param name="linUserIdentity"></param>
        /// <param name="newpassword"></param>
        /// <returns></returns>
        Task ChangePasswordAsync(LinUserIdentity linUserIdentity, string newpassword);
        
        /// <summary>
        /// 根据用户ID，修改用户的密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newpassword"></param>
        Task ChangePasswordAsync(long userId, string newpassword);

        Task DeleteAsync(long userId);
        /// <summary>
        /// 根据用户id得到密码模式的用户授权信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<LinUserIdentity> GetFirstByUserIdAsync(long userId);

        Task<List<UserIdentityDto>> GetListAsync(long userId);
        Task<UnifyResponseDto> BindGitHubAsync(ClaimsPrincipal principal, string openId, long userId);
        Task<UnifyResponseDto> BindQQAsync(ClaimsPrincipal principal, string openId, long userId);
        Task<UnifyResponseDto> BindGiteeAsync(ClaimsPrincipal principal, string openId, long userId);
        Task UnBind(Guid id);
    }
}
