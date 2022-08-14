using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Entities;

namespace LinCms.Cms.Users
{
    /// <summary>
    /// 用户对应多个身份体系
    /// </summary>
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

        /// <summary>
        /// 根据用户Id删除绑定信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task DeleteAsync(long userId);

        /// <summary>
        /// 根据用户id得到密码模式的用户授权信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<LinUserIdentity> GetFirstByUserIdAsync(long userId);

        /// <summary>
        /// 根据用户Id获取第三方绑定信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<UserIdentityDto>> GetListAsync(long userId);

        /// <summary>
        /// 解绑用户的第三方账号：当用户没有密码时，无法解绑最后一个账号,只可以解绑自己的账号
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task UnBind(Guid id);
    }
}
