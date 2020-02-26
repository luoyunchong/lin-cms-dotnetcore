using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinCms.Application.Contracts.Cms.Admins;
using LinCms.Application.Contracts.Cms.Users;
using LinCms.Core.Data;
using LinCms.Core.Data.Enums;
using LinCms.Core.Entities;

namespace LinCms.Application.Cms.Users
{
    public interface IUserService
    {
        /// <summary>
        /// 验证条件
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        Task<LinUser> GetUserAsync(Expression<Func<LinUser, bool>> expression);

        /// <summary>
        /// 后台管理员修改用户密码
        /// </summary>
        /// <param name="passwordDto"></param>
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
        /// <param name="userActive"></param>
        Task ChangeStatusAsync(int id, UserActive userActive);

        /// <summary>
        /// 注册-新增一个用户
        /// </summary>
        /// <param name="user"></param>
        Task Register(LinUser user);

        void UpdateUserInfo(int id, UpdateUserDto updateUserDto);

        Task DeleteAsync(int id);

        void ResetPassword(int id, ResetPasswordDto resetPasswordDto);

        bool CheckPermission(int userId, string permission);

        /// <summary>
        /// 得到当前用户上下文
        /// </summary>
        /// <returns></returns>
        LinUser GetCurrentUser();
    }
}