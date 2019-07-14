using LinCms.Web.Models.Admins;
using LinCms.Web.Services.Interfaces;
using LinCms.Zero.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using LinCms.Zero.Data;

namespace LinCms.Web.Controllers
{
    [Route("cms/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IUserSevice _userSevice;

        public AdminController(IUserSevice userSevice)
        {
            _userSevice = userSevice;
        }

        [HttpGet("users")]
        public PagedResultDto<LinUser> GetUserList([FromQuery]UserSearchDto searchDto)
        {
            return _userSevice.GetUserList(searchDto);
        }

        [HttpPut("{id}")]
        public ResultDto Put(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            return _userSevice.UpdateUserInfo(id,updateUserDto);
        }


    }
}