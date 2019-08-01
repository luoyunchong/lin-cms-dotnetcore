using System.Collections.Generic;
using LinCms.Web.Models.Cms.Admins;
using LinCms.Web.Models.Cms.Users;
using LinCms.Zero.Data;
using LinCms.Zero.Data.Enums;
using LinCms.Zero.Dependency;
using LinCms.Zero.Domain;

namespace LinCms.Web.Services.Interfaces
{
    public interface IUserSevice
    {
        LinUser Authorization(string username, string password);
        void ChangePassword(ChangePasswordDto passwordDto);
        PagedResultDto<UserDto> GetUserList(UserSearchDto searchDto);
        void ChangeStatus(int id, UserActive userActive);
        void Register(LinUser user);
        void UpdateUserInfo(int id,UpdateUserDto updateUserDto);
        void Delete(int id);
        void ResetPassword(int id,ResetPasswordDto resetPasswordDto);
        bool CheckPermission(int userId, string permission);
    }
}