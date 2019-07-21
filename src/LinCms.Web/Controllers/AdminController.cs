using LinCms.Web.Models.Admins;
using LinCms.Web.Services.Interfaces;
using LinCms.Zero.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using LinCms.Web.Models.Users;
using LinCms.Zero.Data;
using Microsoft.AspNetCore.Authorization;

namespace LinCms.Web.Controllers
{
    [Route("cms/admin")]
    [ApiController]
    [Authorize(Roles = LinGroup.Administrator)]
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
        public PagedResultDto<LinUser> GetUserList([FromQuery]UserSearchDto searchDto)
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

        [HttpGet("authority")]
        public IActionResult GetAllAuths()
        {
            var r = _freeSql.Select<LinAuth>().ToList();

            return Ok(r);
        }
    }
}