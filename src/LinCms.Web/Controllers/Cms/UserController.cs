using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LinCms.Application.Cms.Users;
using LinCms.Core.Aop;
using LinCms.Core.Data;
using LinCms.Core.Entities;
using LinCms.Core.Security;
using LinCms.Web.Data;
using LinCms.Application.Contracts.Cms.Users;
using LinCms.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.Cms
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
        private readonly AuditBaseRepository<LinUser> _userRepository;

        public UserController(IFreeSql freeSql, IMapper mapper, IUserSevice userSevice, ICurrentUser currentUser, AuditBaseRepository<LinUser> userRepository)
        {
            _freeSql = freeSql;
            _mapper = mapper;
            _userSevice = userSevice;
            _currentUser = currentUser;
            _userRepository = userRepository;
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
            linUser.Avatar = _currentUser.GetFileUrl(linUser.Avatar);

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
            user.Avatar = _currentUser.GetFileUrl(linUser.Avatar);
            user.GroupName = user.GroupId != null ? _freeSql.Select<LinGroup>().Where(r => r.Id == user.GroupId).First()?.Info : "";
            if (linUser.IsAdmin())
            {
                user.Auths = new List<IDictionary<string, object>>();
            }
            else
            {
                if (linUser.GroupId != 0)
                {
                    List<LinAuth> listAuths = _freeSql.Select<LinAuth>().Where(r => r.GroupId == linUser.GroupId).ToList();

                    user.Auths = ReflexHelper.AuthsConvertToTree(listAuths); ;

                }
            }

            return user;
        }

        /// <summary>
        /// 新增用户-不是注册，注册不可能让用户选择gourp_id
        /// </summary>
        /// <param name="userInput"></param>
        [AuditingLog("管理员新建了一个用户")]
        [HttpPost]
        [LinCmsAuthorize(Roles = LinGroup.Admin)]
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

        [HttpPut("nickname")]
        public ResultDto SetNickname(UpdateNicknameDto updateNicknameDto)
        {
            _freeSql.Update<LinUser>(_currentUser.Id).Set(a => new LinUser()
            {
                Nickname = updateNicknameDto.Nickname
            }).ExecuteAffrows();
            return ResultDto.Success("更新昵称成功");
        }

        [HttpPut]
        public ResultDto SetProfileInfo(UpdateProfileDto updateProfileDto)
        {
            _freeSql.Update<LinUser>(_currentUser.Id).Set(a => new LinUser()
            {
                Nickname = updateProfileDto.Nickname,
                Introduction = updateProfileDto.Introduction
            }).ExecuteAffrows();
            return ResultDto.Success("更新基本信息成功");
        }

        [AllowAnonymous]
        [HttpGet("avatar/{userId}")]
        public string GetAvatar(long userId)
        {
            LinUser linUser = _freeSql.Select<LinUser>().WhereCascade(r => r.IsDeleted == false).Where(r => r.Id == userId).First();

            return _currentUser.GetFileUrl(linUser.Avatar);

        }

        [AllowAnonymous]
        [HttpGet("{userId}")]
        public OpenUserDto GetUserByUserId(long userId)
        {
            LinUser linUser = _freeSql.Select<LinUser>().WhereCascade(r => r.IsDeleted == false).Where(r => r.Id == userId).First();
            OpenUserDto openUser = _mapper.Map<LinUser, OpenUserDto>(linUser);
            if (openUser == null) return null;
            openUser.Avatar = _currentUser.GetFileUrl(openUser.Avatar);

            return openUser;

        }

        [AllowAnonymous]
        [HttpGet("novices")]
        public List<UserNoviceDto> GetNovices()
        {
            List<UserNoviceDto> userNoviceDtos = _userRepository.Select.OrderByDescending(r => r.CreateTime).Take(12)
                .ToList(r => new UserNoviceDto()
                {
                    Id = r.Id,
                    Introduction = r.Introduction,
                    Nickname = r.Nickname,
                    Avatar = r.Avatar,
                    Username = r.Username,
                    LastLoginTime = r.LastLoginTime,
                    CreateTime = r.CreateTime,
                }).Select(r =>
                {
                    r.Avatar = _currentUser.GetFileUrl(r.Avatar);
                    return r;
                }).ToList();

            return userNoviceDtos;
        }
    }
}
