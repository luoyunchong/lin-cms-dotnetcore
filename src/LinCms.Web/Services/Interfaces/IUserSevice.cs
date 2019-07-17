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
        ResultDto Register(LinUser user);
        ResultDto UpdateUserInfo(int id,UpdateUserDto updateUserDto);
        ResultDto Delete(int id);
    }
}