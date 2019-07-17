using System.Collections.Generic;
using AutoMapper;
using IdentityModel;
using LinCms.Web.Models.Users;
using LinCms.Zero.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using LinCms.Web.Services.Interfaces;
using LinCms.Zero.Data;

namespace LinCms.Web.Controllers
{
    [ApiController]
    [Route("cms/user")]
    //[Authorize]
    public class UserController : ControllerBase
    {
        private readonly IFreeSql _freeSql;
        private readonly IMapper _mapper;
        private readonly IUserSevice _userSevice;

        public UserController(IFreeSql freeSql, IMapper mapper, IUserSevice userSevice)
        {
            _freeSql = freeSql;
            _mapper = mapper;
            _userSevice = userSevice;
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
            string id = User.FindFirst(JwtClaimTypes.Id)?.Value;

            LinUser linUser = _freeSql.Select<LinUser>().Where(r => r.Id == int.Parse(id)).First();

            return _mapper.Map<UserInformation>(linUser);

            //return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
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
        public ResultDto Post([FromBody] UserInputDto userInput)
        {
            return _userSevice.Register(_mapper.Map<LinUser>(userInput));
        }


        [HttpPost("change_password")]
        public ResultDto ChangePassword([FromBody] ChangePasswordDto passwordDto)
        {
            bool ok = _userSevice.ChangePassword(passwordDto);

            return ok ? ResultDto.Success("密码修改成功") : ResultDto.Error("修改密码失败");
        }
    }
}
