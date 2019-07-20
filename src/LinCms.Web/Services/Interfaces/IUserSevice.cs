using System.Collections.Generic;
using LinCms.Web.Models.Admins;
using LinCms.Web.Models.Users;
using LinCms.Zero.Data;
using LinCms.Zero.Dependency;
using LinCms.Zero.Domain;

namespace LinCms.Web.Services.Interfaces
{
    public interface IUserSevice
    {
        LinUser Authorization(string username, string password);
        bool ChangePassword(ChangePasswordDto passwordDto);
        PagedResultDto<LinUser> GetUserList(UserSearchDto searchDto);
        void Register(LinUser user);
        void UpdateUserInfo(int id,UpdateUserDto updateUserDto);
        void Delete(int id);
        void ResetPassword(int id,ResetPasswordDto resetPasswordDto);
    }
}