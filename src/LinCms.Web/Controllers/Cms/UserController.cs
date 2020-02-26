using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Application.Cms.Users;
using LinCms.Application.Contracts.Cms.Groups;
using LinCms.Core.Aop;
using LinCms.Core.Data;
using LinCms.Core.Entities;
using LinCms.Core.Security;
using LinCms.Web.Data;
using LinCms.Application.Contracts.Cms.Users;
using LinCms.Core.Exceptions;
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
        private readonly IUserService _userSevice;
        private readonly ICurrentUser _currentUser;
        private readonly UserRepository _userRepository;

        public UserController(IFreeSql freeSql, IMapper mapper, IUserService userSevice, ICurrentUser currentUser, UserRepository userRepository)
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
        public async Task<UserInformation> GetInformationAsync()
        {
            LinUser linUser = await _userSevice.GetUserAsync(r => r.Id == _currentUser.Id);
            linUser.Avatar = _currentUser.GetFileUrl(linUser.Avatar);

            UserInformation userInformation = _mapper.Map<UserInformation>(linUser);
            userInformation.Groups = linUser.LinGroups.Select(r => _mapper.Map<GroupDto>(r)).ToList();

            return userInformation;
        }

        /// <summary>
        /// 查询自己拥有的权限
        /// </summary>
        /// <returns></returns>
        [HttpGet("permissions")]
        public async Task<UserInformation> Permissions()
        {
            UserInformation user = await this.GetInformationAsync();

            if (user.Groups.Count != 0)
            {
                var groupIds = user.Groups.Select(r => r.Id).ToArray();

                List<LinPermission> listAuths = _freeSql.Select<LinPermission>().Where(a => groupIds.Contains(a.GroupId)).ToList();

                user.Auths = ReflexHelper.AuthsConvertToTree(listAuths); ;

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

        [AuditingLog("修改了自己的密码")]
        [HttpPut("change_password")]
        public async Task<ResultDto> ChangePasswordAsync([FromBody] ChangePasswordDto passwordDto)
        {
            await _userSevice.ChangePasswordAsync(passwordDto);

            return ResultDto.Success("密码修改成功");
        }

        [HttpPut("avatar")]
        public async Task<ResultDto> SetAvatar(UpdateAvatarDto avatarDto)
        {
            await _freeSql.Update<LinUser>(_currentUser.Id).Set(a => new LinUser()
            {
                Avatar = avatarDto.Avatar
            }).ExecuteAffrowsAsync();

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
