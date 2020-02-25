using System.Security.Claims;
using System.Threading.Tasks;
using LinCms.Application.Contracts.Cms.Users;

namespace LinCms.Application.Cms.Users
{
 
    public interface IUserIdentityService
    {
        long SaveGitHub(ClaimsPrincipal principal, string openId);
        /// <summary>
        /// 验证用户密码是否正确
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        bool VerifyUsernamePassword(long userId, string username, string password);


        /// <summary>
        /// 修改某用户的密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newpassword"></param>
        Task ChangePasswordAsync(long userId, string newpassword);
    }
}
