using System.Collections.Generic;
using LinCms.Web.Data;
using LinCms.Web.Data.Aop;
using LinCms.Web.Data.Authorization;
using LinCms.Web.Models.Admins;
using LinCms.Web.Models.Users;
using LinCms.Web.Services.Interfaces;
using LinCms.Zero.Data;
using LinCms.Zero.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.Cms
{
    [Route("cms/admin")]
    [ApiController]
    [LinCmsAuthorize(Roles = LinGroup.Administrator)]
    public class AdminController : ControllerBase
    {
        private readonly IUserSevice _userSevice;
        private readonly IFreeSql _freeSql;

        public AdminController(IUserSevice userSevice, IFreeSql freeSql)
        {
            _userSevice = userSevice;
            _freeSql = freeSql;
        }

        /// <summary>
        /// 用户信息分页列表项
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        [HttpGet("users")]
        public PagedResultDto<UserDto> GetUserList([FromQuery]UserSearchDto searchDto)
        {
            return _userSevice.GetUserList(searchDto);
        }

        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateUserDto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public ResultDto Put(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            _userSevice.UpdateUserInfo(id, updateUserDto);
            return ResultDto.Success();
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AuditingLog("管理员删除了一个用户")]
        [HttpDelete("{id}")]
        public ResultDto Delete(int id)
        {
            _userSevice.Delete(id);
            return ResultDto.Success();
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="resetPasswordDto"></param>
        /// <returns></returns>
        [HttpPut("password/{id}")]
        public ResultDto ResetPassword(int id, [FromBody] ResetPasswordDto resetPasswordDto)
        {
            _userSevice.ResetPassword(id, resetPasswordDto);
            return ResultDto.Success("密码修改成功");
        }

        /// <summary>
        /// 查询所有可分配的权限
        /// </summary>
        /// <returns></returns>
        [HttpGet("authority")]
        public IActionResult GetAllAuths()
        {
            List<PermissionDto> linCmsAttributes = ReflexHelper.GeAssemblyLinCmsAttributes();

            dynamic obj = ReflexHelper.AuthorizationConvertToTree(linCmsAttributes);

            return Ok(obj);
        }
    }
}