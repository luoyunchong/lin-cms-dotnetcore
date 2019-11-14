using LinCms.Web.Models.Cms.Admins;
using LinCms.Web.Models.Cms.Users;
using LinCms.Zero.Data;
using LinCms.Zero.Data.Enums;
using LinCms.Zero.Domain;

namespace LinCms.Web.Services.Cms.Interfaces
{
    public interface IUserSevice
    {
        /// <summary>
        /// 验证用户名密码是否正确
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        LinUser Authorization(string username, string password);
        /// <summary>
        /// 后台管理员修改用户密码
        /// </summary>
        /// <param name="passwordDto"></param>
        void ChangePassword(ChangePasswordDto passwordDto);
        /// <summary>
        /// 根据查询条件查询用户信息
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        PagedResultDto<UserDto> GetUserList(UserSearchDto searchDto);
        /// <summary>
        /// 修改用户状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userActive"></param>
        void ChangeStatus(int id, UserActive userActive);
        /// <summary>
        /// 注册-新增一个用户
        /// </summary>
        /// <param name="user"></param>
        void Register(LinUser user);
        void UpdateUserInfo(int id,UpdateUserDto updateUserDto);
        void Delete(int id);
        void ResetPassword(int id,ResetPasswordDto resetPasswordDto);
        bool CheckPermission(int userId, string permission);
        /// <summary>
        /// 得到当前用户上下文
        /// </summary>
        /// <returns></returns>
        LinUser GetCurrentUser();
    }
}