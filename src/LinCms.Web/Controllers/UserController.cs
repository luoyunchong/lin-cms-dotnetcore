using AutoMapper;
using IdentityModel;
using LinCms.Web.Models.Users;
using LinCms.Web.Services.Interfaces;
using LinCms.Zero.Data;
using LinCms.Zero.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using LinCms.Zero.Authorization;
using LinCms.Zero.Security;
using Microsoft.AspNetCore.Authorization;

namespace LinCms.Web.Controllers
{
    [ApiController]
    [Route("cms/user")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IFreeSql _freeSql;
        private readonly IMapper _mapper;
        private readonly IUserSevice _userSevice;
        private readonly ICurrentUser _currentUser;

        public UserController(IFreeSql freeSql, IMapper mapper, IUserSevice userSevice, ICurrentUser currentUser)
        {
            _freeSql = freeSql;
            _mapper = mapper;
            _userSevice = userSevice;
            _currentUser = currentUser;
        }

        [HttpGet("get")]
        public JsonResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }

        /// <summary>
        /// 得到当前登录人信息
        /// </summary>
        [HttpGet("information")]
        public UserInformation GetInformation()
        {
            LinUser linUser = _freeSql.Select<LinUser>().Where(r => r.Id == _currentUser.Id).First();

            return _mapper.Map<UserInformation>(linUser);
        }

        [HttpGet("auths")]
        public void Auths()
        {
            ;
        }

        /// <summary>
        /// 新增用户-不是注册，注册不可能让用户选择gourp_id
        /// </summary>
        /// <param name="userInput"></param>
        [HttpPost("register")]
        public ResultDto Post([FromBody] CreateUserDto userInput)
        {
            _userSevice.Register(_mapper.Map<LinUser>(userInput));

            return ResultDto.Success("用户创建成功");
        }


        [HttpPost("change_password")]
        public ResultDto ChangePassword([FromBody] ChangePasswordDto passwordDto)
        {
            _userSevice.ChangePassword(passwordDto);

            return ResultDto.Success("密码修改成功");
        }

    }
}
