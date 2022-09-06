using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Aop.Filter;
using LinCms.Cms.Admins;
using LinCms.Cms.Users;
using LinCms.Data;
using LinCms.Data.Enums;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Controllers.Cms;

[ApiExplorerSettings(GroupName = "cms")]
[Route("cms/admin")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly IUserService _userSevice;
    private readonly IAdminService _adminService;
    public AdminController(IUserService userSevice, IAdminService adminService)
    {
        _userSevice = userSevice;
        _adminService = adminService;
    }

    /// <summary>
    /// 用户信息分页列表项
    /// </summary>
    /// <param name="searchDto"></param>
    /// <returns></returns>
    [HttpGet("users")]
    [LinCmsAuthorize("查询所有用户", "管理员")]
    public PagedResultDto<UserDto> GetUserListByGroupId([FromQuery] UserSearchDto searchDto)
    {
        return _userSevice.GetUserListByGroupId(searchDto);
    }

    /// <summary>
    /// 修改用户状态
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userStatus"></param>
    /// <returns></returns>
    [HttpPut("user/{id}/status/{userStatus}")]
    [LinCmsAuthorize("修改用户密码", "管理员")]
    public Task ChangeStatusAsync(long id, UserStatus userStatus)
    {
        return _userSevice.ChangeStatusAsync(id, userStatus);
    }

    /// <summary>
    /// 修改用户信息
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updateUserDto"></param>
    /// <returns></returns>
    [HttpPut("user/{id}")]
    [LinCmsAuthorize("管理员更新用户信息", "管理员")]
    public async Task<UnifyResponseDto> UpdateAsync(long id, [FromBody] UpdateUserDto updateUserDto)
    {
        await _userSevice.UpdateAync(id, updateUserDto);
        return UnifyResponseDto.Success();
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Logger("管理员删除了一个用户")]
    [HttpDelete("user/{id}")]
    [LinCmsAuthorize("删除用户", "管理员")]
    public async Task<UnifyResponseDto> DeleteAsync(long id)
    {
        await _userSevice.DeleteAsync(id);
        return UnifyResponseDto.Success("删除用户成功");
    }

    /// <summary>
    /// 重置密码
    /// </summary>
    /// <param name="id">用户id</param>
    /// <param name="resetPasswordDto"></param>
    /// <returns></returns>
    [HttpPut("user/{id}/password")]
    [LinCmsAuthorize("修改用户密码", "管理员")]
    public async Task<UnifyResponseDto> ResetPasswordAsync(long id, [FromBody] ResetPasswordDto resetPasswordDto)
    {
        await _userSevice.ResetPasswordAsync(id, resetPasswordDto);
        return UnifyResponseDto.Success("密码修改成功");
    }

}