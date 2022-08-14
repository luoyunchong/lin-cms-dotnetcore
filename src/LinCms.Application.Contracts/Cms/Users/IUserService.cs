using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Cms.Admins;
using LinCms.Data;
using LinCms.Data.Enums;
using LinCms.Entities;

namespace LinCms.Cms.Users
{
    /// <summary>
    /// 用户服务 
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 用户修改了自己的密码
        /// </summary>
        /// <param name="passwordDto"></param>
        /// <returns></returns>
        Task ChangePasswordAsync(ChangePasswordDto passwordDto);

        /// <summary>
        /// 根据分组条件查询用户信息
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        PagedResultDto<UserDto> GetUserListByGroupId(UserSearchDto searchDto);

        /// <summary>
        /// 修改用户状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userStatus"></param>
        Task ChangeStatusAsync(long id, UserStatus userStatus);

        /// <summary>
        /// 注册-新增一个用户
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="groupIds">分组Id集合</param>
        /// <param name="password">密码</param>
        Task CreateAsync(LinUser user, List<long> groupIds, string password);

        Task UpdateAync(long id, UpdateUserDto updateUserDto);

        Task DeleteAsync(long id);

        /// <summary>
        /// 后台管理员重置用户密码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="resetPasswordDto"></param>
        Task ResetPasswordAsync(long id, ResetPasswordDto resetPasswordDto);

        /// <summary>
        /// 得到当前用户上下文
        /// </summary>
        /// <returns></returns>
        Task<LinUser> GetCurrentUserAsync();

        /// <summary>
        /// 根据用户Id获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<UserInformation> GetInformationAsync(long userId);

        /// <summary>
        /// 查询用户拥有的权限
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<IDictionary<string, object>>> GetStructualUserPermissions(long userId);

        /// <summary>
        /// 查找用户搜索分组，查找分组下的所有权限
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<LinPermission>> GetUserPermissionsAsync(long userId);

    }
}