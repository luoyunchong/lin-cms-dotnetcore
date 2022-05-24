using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Entities;

namespace LinCms.Cms.Users
{

    public interface IUserIdentityService
    {
        /// <summary>
        /// 验证用户密码是否正确
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        Task<bool> VerifyUserPasswordAsync(long userId, string password, string salt);

        /// <summary>
        /// 根据linUserIdentity生成密码
        /// </summary>
        /// <param name="linUserIdentity"></param>
        /// <param name="newpassword"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        Task ChangePasswordAsync(LinUserIdentity linUserIdentity, string newpassword, string salt);

        /// <summary>
        /// 根据用户ID，修改用户的密码
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newpassword"></param>
        /// <param name="salt"></param>
        Task ChangePasswordAsync(long userId, string newpassword, string salt);

        Task DeleteAsync(long userId);
        /// <summary>
        /// 根据用户id得到密码模式的用户授权信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<LinUserIdentity> GetFirstByUserIdAsync(long userId);

        Task<List<UserIdentityDto>> GetListAsync(long userId);
        Task UnBind(Guid id);
    }
}
