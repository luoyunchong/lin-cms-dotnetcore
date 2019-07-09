using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Web.Areas.Cms.Models.Users;
using LinCms.Web.Domain;
using LinCms.Web.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Areas.Cms.Controllers
{
    [ApiController]
    [Route("cms/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        /// <summary>
        /// 注册接口
        /// </summary>
        /// <param name="value"></param>
        [HttpPost("register")]
        public void Post([FromBody] string value)
        {
        }

        /// <summary>
        /// 登录接口
        /// </summary>
        /// <param name="loginInputDto"></param>
        [HttpPost("login")]
        public string Login(LoginInputDto loginInputDto)
        {
            return _userService.Authenticate(loginInputDto.Nickname, loginInputDto.Password);
        }

        /// <summary>
        /// 得到当前登录人信息
        /// </summary>
        [HttpGet("information")]
        public void GetInformation()
        {
        }

        /// <summary>
        /// 刷新用户的token
        /// </summary>
        /// <returns></returns>
        [HttpGet("refresh")]
        public string GetRefreshToken()
        {
            return "";
        }
    }
}
