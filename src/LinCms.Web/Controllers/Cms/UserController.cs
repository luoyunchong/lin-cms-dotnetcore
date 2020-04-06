using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Application.Cms.Users;
using LinCms.Core.Aop;
using LinCms.Core.Data;
using LinCms.Core.Entities;
using LinCms.Core.Security;
using LinCms.Application.Contracts.Cms.Users;
using LinCms.Application.Contracts.Cms.Users.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LinCms.Core.IRepositories;

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
        private readonly IUserRepository _userRepository;

        public UserController(IFreeSql freeSql, IMapper mapper, IUserService userSevice, ICurrentUser currentUser, IUserRepository userRepository)
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
        /// 新增用户-不是注册，注册不可能让用户选择gourp_id
        /// </summary>
        /// <param name="userInput"></param>
        [AuditingLog("管理员新建了一个用户")]
        [HttpPost("register")]
        [LinCmsAuthorize(Roles = LinGroup.Admin)]
        public UnifyResponseDto Post([FromBody] CreateUserDto userInput)
        {
            _userSevice.Register(_mapper.Map<LinUser>(userInput), userInput.GroupIds,userInput.Password);

            return UnifyResponseDto.Success("用户创建成功");
        }
        /// <summary>
        /// 得到当前登录人信息
        /// </summary>
        [HttpGet("information")]
        public async Task<UserInformation> GetInformationAsync()
        {
            UserInformation userInformation = await _userSevice.GetInformationAsync(_currentUser.Id ?? 0);
            return userInformation;
        }

        /// <summary>
        /// 查询自己拥有的权限
        /// </summary>
        /// <returns></returns>
        [HttpGet("permissions")]
        public async Task<UserInformation> Permissions()
        {
            UserInformation userInformation = await _userSevice.GetInformationAsync(_currentUser.Id ?? 0);
            var permissions = await _userSevice.GetStructualUserPermissions(_currentUser.Id ?? 0);
            userInformation.Permissions = permissions;
            return userInformation;
        }

        [AuditingLog("修改了自己的密码")]
        [HttpPut("change_password")]
        public async Task<UnifyResponseDto> ChangePasswordAsync([FromBody] ChangePasswordDto passwordDto)
        {
            await _userSevice.ChangePasswordAsync(passwordDto);

            return UnifyResponseDto.Success("密码修改成功");
        }

        [HttpPut("avatar")]
        public async Task<UnifyResponseDto> SetAvatar(UpdateAvatarDto avatarDto)
        {
            await _freeSql.Update<LinUser>(_currentUser.Id).Set(a => new LinUser()
            {
                Avatar = avatarDto.Avatar
            }).ExecuteAffrowsAsync();

            return UnifyResponseDto.Success("更新头像成功");
        }

        [HttpPut("nickname")]
        public UnifyResponseDto SetNickname(UpdateNicknameDto updateNicknameDto)
        {
            _freeSql.Update<LinUser>(_currentUser.Id).Set(a => new LinUser()
            {
                Nickname = updateNicknameDto.Nickname
            }).ExecuteAffrows();
            return UnifyResponseDto.Success("更新昵称成功");
        }

        [HttpPut]
        public UnifyResponseDto SetProfileInfo(UpdateProfileDto updateProfileDto)
        {
            _freeSql.Update<LinUser>(_currentUser.Id).Set(a => new LinUser()
            {
                Nickname = updateProfileDto.Nickname,
                Introduction = updateProfileDto.Introduction
            }).ExecuteAffrows();
            return UnifyResponseDto.Success("更新基本信息成功");
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
