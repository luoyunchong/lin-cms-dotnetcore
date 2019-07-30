using System.Collections.Generic;
using System.Dynamic;
using AutoMapper;
using IdentityModel;
using LinCms.Web.Models.Users;
using LinCms.Web.Services.Interfaces;
using LinCms.Zero.Data;
using LinCms.Zero.Domain;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using LinCms.Web.Data;
using LinCms.Zero.Authorization;
using LinCms.Zero.Security;
using Microsoft.AspNetCore.Authorization;
using LinCms.Web.Data.Aop;
using LinCms.Web.Models.Auths;

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

        /// <summary>
        /// 查询自己拥有的权限
        /// </summary>
        /// <returns></returns>
        [HttpGet("auths")]
        public UserInformation Auths()
        {
            LinUser linUser = _freeSql.Select<LinUser>().Where(r => r.Id == _currentUser.Id).First();

            UserInformation user = _mapper.Map<UserInformation>(linUser);

            if (linUser.IsAdmin())
            {
                user.Auths = new List<IDictionary<string,object>>();
            }
            else
            {
                if (linUser.GroupId != 0)
                {
                    List<LinAuth> listAuths = _freeSql.Select<LinAuth>().Where(r => r.GroupId == linUser.GroupId).ToList();

                    user.Auths= ReflexHelper.AuthsConvertToTree(listAuths);;

                }
            }

            return user;
        }

        /// <summary>
        /// 新增用户-不是注册，注册不可能让用户选择gourp_id
        /// </summary>
        /// <param name="userInput"></param>
        [AuditingLog("管理员新建了一个用户")]
        [HttpPost("register")]
        [LinCmsAuthorize(Roles = LinGroup.Administrator)]
        public ResultDto Post([FromBody] CreateUserDto userInput)
        {
            _userSevice.Register(_mapper.Map<LinUser>(userInput));

            return ResultDto.Success("用户创建成功");
        }

        [AuditingLog("{0}修改了自己的密码")]
        [HttpPut("change_password")]
        public ResultDto ChangePassword([FromBody] ChangePasswordDto passwordDto)
        {
            _userSevice.ChangePassword(passwordDto);

            return ResultDto.Success("密码修改成功");
        }

        [HttpPut("avatar")]
        public ResultDto SetAvatar(UpdateAvatarDto avatarDto)
        {
            _freeSql.Update<LinUser>(_currentUser.Id).Set(a => new LinUser()
            {
                Avatar = avatarDto.Avatar
            }).ExecuteAffrows();
            return ResultDto.Success("更新头像成功");
        }
    }
}
