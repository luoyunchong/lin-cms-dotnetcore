using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.Security;
using IGeekFan.FreeKit.Extras.Dto;
using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Cms.Admins;
using LinCms.Cms.Groups;
using LinCms.Cms.Permissions;
using LinCms.Common;
using LinCms.Data.Enums;
using LinCms.Entities;
using LinCms.Exceptions;
using LinCms.Extensions;
using LinCms.IRepositories;
using LinCms.Security;

namespace LinCms.Cms.Users;
public class UserService(IUserRepository userRepository,
        IUserIdentityService userIdentityService,
        IPermissionService permissionService,
        IGroupService groupService,
        IFileRepository fileRepository,
        ICryptographyService cryptographyService)
    : ApplicationService, IUserService
{
    public async Task ChangePasswordAsync(ChangePasswordDto passwordDto)
    {
        long currentUserId = CurrentUser.FindUserId() ?? 0;
        LinUser user = await userRepository.Where(r => r.Id == currentUserId).FirstAsync();

        bool valid = await userIdentityService.VerifyUserPasswordAsync(currentUserId, passwordDto.OldPassword, user.Salt);
        if (!valid)
        {
            throw new LinCmsException("旧密码不正确");
        }
        if (user.Salt.IsNullOrWhiteSpace())
        {
            user.Salt = Guid.NewGuid().ToString();
            await userRepository.UpdateAsync(user);
        }

        await userIdentityService.ChangePasswordAsync(user.Id, passwordDto.NewPassword, user.Salt);
    }

    public Task<LinUser> GetUserAsync(string username)
    {
        return userRepository.Where(r => r.Username == username).FirstAsync();
    }

    [Transactional]
    public async Task DeleteAsync(long userId)
    {
        await userRepository.DeleteAsync(new LinUser() { Id = userId });
        await userIdentityService.DeleteAsync(userId);
        await groupService.DeleteUserGroupAsync(userId);
    }

    public async Task ResetPasswordAsync(long id, ResetPasswordDto resetPasswordDto)
    {
        LinUser user = await userRepository.Where(r => r.Id == id).FirstAsync();

        if (user == null)
        {
            throw new LinCmsException("用户不存在", ErrorCode.NotFound);
        }

        if (user.Salt.IsNullOrWhiteSpace())
        {
            user.Salt = Guid.NewGuid().ToString();
            await userRepository.UpdateAsync(user);
        }

        await userIdentityService.ChangePasswordAsync(id, resetPasswordDto.ConfirmPassword, user.Salt);
    }

    public async Task<PagedResultDto<UserDto>> GetListAsync(UserSearchDto searchDto)
    {
        List<UserDto> linUsers = (await userRepository.Select
            .IncludeMany(r => r.LinGroups)
            .WhereIf(searchDto.GroupId != null, r => r.LinUserGroups.AsSelect().Any(u => u.GroupId == searchDto.GroupId))
            .WhereIf(searchDto.Email.IsNotNullOrWhiteSpace(), r => r.Email.Contains(searchDto.Email))
            .WhereIf(searchDto.Nickname.IsNotNullOrWhiteSpace(), r => r.Nickname.Contains(searchDto.Nickname))
            .WhereIf(searchDto.Username.IsNotNullOrWhiteSpace(), r => r.Username.Contains(searchDto.Username))
            .OrderByDescending(r => r.Id)
            .ToPagerListAsync(searchDto, out long totalCount))
            .Select(r =>
            {
                UserDto userDto = Mapper.Map<UserDto>(r);
                userDto.Groups = Mapper.Map<List<GroupDto>>(r.LinGroups);
                return userDto;
            }).ToList();

        return new PagedResultDto<UserDto>(linUsers, totalCount);
    }

    [Transactional]
    public async Task CreateAsync(LinUser user, List<long> groupIds, string password)
    {
        if (!string.IsNullOrEmpty(user.Username))
        {
            bool isRepeatName = await userRepository.Select.AnyAsync(r => r.Username == user.Username);

            if (isRepeatName)
            {
                throw new LinCmsException("用户名重复，请重新输入", ErrorCode.RepeatField);
            }
        }

        if (!string.IsNullOrEmpty(user.Email.Trim()))
        {
            var isRepeatEmail = await userRepository.Select.AnyAsync(r => r.Email == user.Email.Trim());
            if (isRepeatEmail)
            {
                throw new LinCmsException("注册邮箱重复，请重新输入", ErrorCode.RepeatField);
            }
        }
        user.Salt = Guid.NewGuid().ToString();
        string encryptPassword = cryptographyService.Encrypt(password, user.Salt);
        user.LinUserGroups = new List<LinUserGroup>();
        groupIds?.ForEach(groupId =>
        {
            user.LinUserGroups.Add(new LinUserGroup()
            {
                GroupId = groupId
            });
        });
        user.LinUserIdentitys = new List<LinUserIdentity>()
        {
            new(LinUserIdentity.Password,user.Username,encryptPassword,DateTime.Now)
        };
        await userRepository.InsertAsync(user);
    }

    /// <summary>
    /// 修改指定字段，邮件和组别
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updateUserDto"></param>    
    /// <returns></returns>
    [Transactional]
    public async Task UpdateAync(long id, UpdateUserDto updateUserDto)
    {
        LinUser linUser = await userRepository.Where(r => r.Id == id).ToOneAsync();
        if (linUser == null)
        {
            throw new LinCmsException("用户不存在", ErrorCode.NotFound);
        }

        if (!string.IsNullOrEmpty(updateUserDto.Username))
        {
            bool isRepeatName = await userRepository.Select.AnyAsync(r => r.Username == updateUserDto.Username && r.Id != id);

            if (isRepeatName)
            {
                throw new LinCmsException("用户名重复，请重新输入", ErrorCode.RepeatField);
            }
        }

        if (!string.IsNullOrEmpty(updateUserDto.Email?.Trim()))
        {
            var isRepeatEmail = await userRepository.Select.AnyAsync(r => r.Email == updateUserDto.Email.Trim() && r.Id != id);
            if (isRepeatEmail)
            {
                throw new LinCmsException("注册邮箱重复，请重新输入", ErrorCode.RepeatField);
            }
        }

        List<long> existGroupIds = await groupService.GetGroupIdsByUserIdAsync(id);

        //删除existGroupIds有，而newGroupIds没有的
        List<long> deleteIds = existGroupIds.Where(r => !updateUserDto.GroupIds.Contains(r)).ToList();

        //添加newGroupIds有，而existGroupIds没有的
        List<long> addIds = updateUserDto.GroupIds.Where(r => !existGroupIds.Contains(r)).ToList();

        Mapper.Map(updateUserDto, linUser);
        await userRepository.UpdateAsync(linUser);
        await groupService.DeleteUserGroupAsync(id, deleteIds);
        await groupService.AddUserGroupAsync(id, addIds);
    }

    public async Task ChangeStatusAsync(long id, UserStatus userStatus)
    {
        LinUser user = await userRepository.Select.Where(r => r.Id == id).ToOneAsync();

        if (user == null)
        {
            throw new LinCmsException("用户不存在", ErrorCode.NotFound);
        }

        if (user.IsActive() && userStatus == UserStatus.Active)
        {
            throw new LinCmsException("当前用户已处于禁止状态");
        }

        if (!user.IsActive() && userStatus == UserStatus.NotActive)
        {
            throw new LinCmsException("当前用户已处于激活状态");
        }

        await userRepository.UpdateDiy.Where(r => r.Id == id)
            .Set(r => new { Active = userStatus.GetHashCode() })
            .ExecuteUpdatedAsync();
    }

    public Task<LinUser> GetCurrentUserAsync()
    {
        if (CurrentUser.FindUserId() != null)
        {
            return userRepository.Select.Where(r => r.Id == CurrentUser.FindUserId()).ToOneAsync();
        }
        return null;
    }

    public async Task<UserInformation> GetInformationAsync(long userId)
    {
        LinUser linUser = await userRepository.GetUserAsync(r => r.Id == userId);
        if (linUser == null) return null;
        linUser.Avatar = fileRepository.GetFileUrl(linUser.Avatar);

        UserInformation userInformation = Mapper.Map<UserInformation>(linUser);
        userInformation.Groups = linUser.LinGroups.Select(r => Mapper.Map<GroupDto>(r)).ToList();
        userInformation.Admin = CurrentUser.IsInGroup(LinConsts.Group.Admin);

        return userInformation;
    }

    public async Task<List<IDictionary<string, object>>> GetStructualUserPermissions(long userId)
    {
        List<LinPermission> permissions = await GetUserPermissionsAsync(userId);
        return permissionService.StructuringPermissions(permissions);
    }

    /// <summary>
    /// 查找用户搜索分组，查找分组下的所有权限
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<List<LinPermission>> GetUserPermissionsAsync(long userId)
    {
        LinUser linUser = await userRepository.GetUserAsync(r => r.Id == userId);
        List<long> groupIds = linUser.LinGroups.Select(r => r.Id).ToList();
        if (linUser.LinGroups == null || linUser.LinGroups.Count == 0)
        {
            return new List<LinPermission>();
        }
        return await permissionService.GetPermissionByGroupIds(groupIds);
    }
}
