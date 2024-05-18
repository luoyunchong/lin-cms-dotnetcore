using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IGeekFan.FreeKit.Extras;
using IGeekFan.FreeKit.Extras.FreeSql;
using IGeekFan.FreeKit.Extras.Security;
using LinCms.Cms.Groups;
using LinCms.Cms.Users;
using LinCms.Data;
using LinCms.Entities;
using LinCms.IRepositories;
using LinCms.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Controllers.Cms;


/// <summary>
/// 用户
/// </summary>
[ApiExplorerSettings(GroupName = "cms")]
[ApiController]
[Route("cms/user")]
[Authorize]
public class UserController(IFreeSql freeSql, 
        IMapper mapper, 
        IUserService userSevice, 
        ICurrentUser currentUser,
        IUserRepository userRepository,
        IGroupService groupService, 
        IFileRepository fileRepository)
    : ControllerBase
{
    [HttpGet("get")]
    public JsonResult Get()
    {
        return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
    }

    /// <summary>
    /// 新增用户-不是注册，注册不可能让用户选择group_id
    /// </summary>
    /// <param name="userInput"></param>
    [Logger("管理员新建了一个用户")]
    [HttpPost("register")]
    [Authorize(Roles = LinGroup.Admin)]
    public async Task<UnifyResponseDto> CreateAsync([FromBody] CreateUserDto userInput)
    {
        await userSevice.CreateAsync(mapper.Map<LinUser>(userInput), userInput.GroupIds, userInput.Password);
        return UnifyResponseDto.Success("用户创建成功");
    }

    /// <summary>
    /// 得到当前登录人信息
    /// </summary>
    [HttpGet("information")]
    public Task<UserInformation> GetInformationAsync()
    {
        return userSevice.GetInformationAsync(currentUser.FindUserId() ?? 0);
    }

    /// <summary>
    /// 查询自己拥有的权限
    /// </summary>
    /// <returns></returns>
    [HttpGet("permissions")]
    public async Task<UserInformation> Permissions()
    {
        UserInformation userInformation = await userSevice.GetInformationAsync(currentUser.FindUserId() ?? 0);
        var permissions = await userSevice.GetStructualUserPermissions(currentUser.FindUserId() ?? 0);
        userInformation.Permissions = permissions;
        userInformation.Admin = groupService.CheckIsRootByUserId(currentUser.FindUserId() ?? 0);
        return userInformation;
    }

    /// <summary>
    /// 修改自己的密码
    /// </summary>
    /// <param name="passwordDto"></param>
    /// <returns></returns>
    [Logger("修改自己的密码")]
    [HttpPut("change_password")]
    public async Task<UnifyResponseDto> ChangePasswordAsync([FromBody] ChangePasswordDto passwordDto)
    {
        await userSevice.ChangePasswordAsync(passwordDto);
        return UnifyResponseDto.Success("密码修改成功");
    }

    /// <summary>
    /// 设置自己的头像
    /// </summary>
    /// <param name="avatarDto"></param>
    /// <returns></returns>
    [HttpPut("avatar")]
    public async Task<UnifyResponseDto> SetAvatar(UpdateAvatarDto avatarDto)
    {
        await freeSql.Update<LinUser>(currentUser.FindUserId()).Set(a => new LinUser()
        {
            Avatar = avatarDto.Avatar
        }).ExecuteAffrowsAsync();

        return UnifyResponseDto.Success("更新头像成功");
    }

    /// <summary>
    /// 修改自己的昵称
    /// </summary>
    /// <param name="updateNicknameDto"></param>
    /// <returns></returns>
    [Logger("修改自己的昵称")]
    [HttpPut("nickname")]
    public UnifyResponseDto SetNickname(UpdateNicknameDto updateNicknameDto)
    {
        freeSql.Update<LinUser>(currentUser.FindUserId()).Set(a => new LinUser()
        {
            Nickname = updateNicknameDto.Nickname
        }).ExecuteAffrows();
        return UnifyResponseDto.Success("更新昵称成功");
    }

    /// <summary>
    /// 修改个人信息
    /// </summary>
    /// <param name="profileDto"></param>
    /// <returns></returns>
    [Logger("修改个人信息")]
    [HttpPut]
    public async Task<UnifyResponseDto> SetProfileInfo(UpdateProfileDto profileDto)
    {
        await freeSql.Update<LinUser>(currentUser.FindUserId()).Set(a => new LinUser()
        {
            Nickname = profileDto.Nickname,
            BlogAddress = profileDto.BlogAddress,
            Introduction = profileDto.Introduction,
            JobTitle = profileDto.JobTitle,
            Company = profileDto.Company
        }).ExecuteAffrowsAsync();
        return UnifyResponseDto.Success("更新基本信息成功");
    }

    /// <summary>
    /// 获取登录人的头像
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("avatar/{userId}")]
    public async Task<string> GetAvatarAsync(long userId)
    {
        string avatar = await userRepository.Where(r => r.Id == userId).FirstAsync(r => r.Avatar);
        return fileRepository.GetFileUrl(avatar);

    }

    /// <summary>
    /// 根据用户id解析出用户开放的基本信息
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet("{userId}")]
    public async Task<OpenUserDto?> GetUserByUserId(long userId)
    {
        LinUser linUser = await userRepository.Where(r => r.Id == userId).FirstAsync();
        OpenUserDto openUser = mapper.Map<LinUser, OpenUserDto>(linUser);
        if (openUser == null) return null;
        openUser.Avatar = fileRepository.GetFileUrl(openUser.Avatar);
        return openUser;
    }

    /// <summary>
    /// 获取最新的12条新手用户数据
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [Cacheable]
    [HttpGet("novices")]
    public virtual async Task<List<UserNoviceDto>> GetNovicesAsync()
    {
        List<UserNoviceDto> userNoviceDtos = (await userRepository.Select
            .OrderByDescending(r => r.CreateTime)
            .Take(12)
            .ToListAsync(r => new UserNoviceDto()
            {
                Id = r.Id,
                Introduction = r.Introduction,
                Nickname = r.Nickname,
                Avatar = r.Avatar,
                Username = r.Username,
                LastLoginTime = r.LastLoginTime,
                CreateTime = r.CreateTime,
            })).Select(r =>
            {
                r.Avatar = fileRepository.GetFileUrl(r.Avatar);
                return r;
            }).ToList();

        return userNoviceDtos;
    }
}