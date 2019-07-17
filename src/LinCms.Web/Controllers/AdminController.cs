using LinCms.Web.Models.Admins;
using LinCms.Web.Services.Interfaces;
using LinCms.Zero.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using LinCms.Zero.Data;
using Microsoft.AspNetCore.Authorization;

namespace LinCms.Web.Controllers
{
    [Route("cms/admin")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly IUserSevice _userSevice;

        public AdminController(IUserSevice userSevice)
        {
            _userSevice = userSevice;
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
            return _userSevice.UpdateUserInfo(id,updateUserDto);
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public  ResultDto Delete(int id)
        {
            return _userSevice.Delete(id);
        }

    }
}