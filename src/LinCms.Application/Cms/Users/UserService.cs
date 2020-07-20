using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Application.Contracts.Cms.Admins.Dtos;
using LinCms.Application.Contracts.Cms.Groups;
using LinCms.Application.Contracts.Cms.Groups.Dtos;
using LinCms.Application.Contracts.Cms.Permissions;
using LinCms.Application.Contracts.Cms.Users;
using LinCms.Application.Contracts.Cms.Users.Dtos;
using LinCms.Core.Aop.Attributes;
using LinCms.Core.Common;
using LinCms.Core.Data;
using LinCms.Core.Data.Enums;
using LinCms.Core.Entities;
using LinCms.Core.Exceptions;
using LinCms.Core.Extensions;
using LinCms.Core.IRepositories;
using LinCms.Core.Security;
using Microsoft.AspNetCore.Authorization;

namespace LinCms.Application.Cms.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        private readonly IUserIdentityService _userIdentityService;
        private readonly IPermissionService _permissionService;
        private readonly IGroupService _groupService;
        private readonly IFileRepository _fileRepository;

        public UserService(IUserRepository userRepository,
            IMapper mapper,
            ICurrentUser currentUser,
            IUserIdentityService userIdentityService,
            IPermissionService permissionService, IGroupService groupService, IFileRepository fileRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _currentUser = currentUser;
            _userIdentityService = userIdentityService;
            _permissionService = permissionService;
            _groupService = groupService;
            _fileRepository = fileRepository;
        }

        public async Task ChangePasswordAsync(ChangePasswordDto passwordDto)
        {
            long currentUserId = _currentUser.Id ?? 0;

            LinUserIdentity userIdentity = await _userIdentityService.GetFirstByUserIdAsync(currentUserId);
            if (userIdentity != null)
            {
                bool valid = EncryptUtil.Verify(userIdentity.Credential, passwordDto.OldPassword);
                if (!valid)
                {
                    throw new LinCmsException("旧密码不正确");
                }
            }

            await _userIdentityService.ChangePasswordAsync(userIdentity, passwordDto.NewPassword);
        }

        public async Task<LinUser> GetUserAsync(string username)
        {
            return await _userRepository.Where(r => r.Username == username).FirstAsync();
        }

        [Transactional]
        public async Task DeleteAsync(long userId)
        {
            await _userRepository.DeleteAsync(new LinUser() { Id = userId });
            await _userIdentityService.DeleteAsync(userId);
            await _groupService.DeleteUserGroupAsync(userId);
        }

        public async Task ResetPasswordAsync(long id, ResetPasswordDto resetPasswordDto)
        {
            bool userExist = await _userRepository.Where(r => r.Id == id).AnyAsync();

            if (userExist == false)
            {
                throw new LinCmsException("用户不存在", ErrorCode.NotFound);
            }

            await _userIdentityService.ChangePasswordAsync(id, resetPasswordDto.ConfirmPassword);
        }

        public PagedResultDto<UserDto> GetUserListByGroupId(UserSearchDto searchDto)
        {
            List<UserDto> linUsers = _userRepository.Select
                .IncludeMany(r => r.LinGroups)
                .WhereIf(searchDto.GroupId != null,r => r.LinUserGroups.AsSelect().Any(u => u.GroupId == searchDto.GroupId))
                .OrderByDescending(r => r.Id)
                .ToPagerList(searchDto, out long totalCount)
                .Select(r =>
                {
                    UserDto userDto = _mapper.Map<UserDto>(r);
                    userDto.Groups = _mapper.Map<List<GroupDto>>(r.LinGroups);
                    return userDto;
                }).ToList();

            return new PagedResultDto<UserDto>(linUsers, totalCount);
        }

        [Transactional]
        public async Task CreateAsync(LinUser user, List<long> groupIds, string password)
        {
            if (!string.IsNullOrEmpty(user.Username))
            {
                bool isRepeatName = await _userRepository.Select.AnyAsync(r => r.Username == user.Username);

                if (isRepeatName)
                {
                    throw new LinCmsException("用户名重复，请重新输入", ErrorCode.RepeatField);
                }
            }

            if (!string.IsNullOrEmpty(user.Email.Trim()))
            {
                var isRepeatEmail = await _userRepository.Select.AnyAsync(r => r.Email == user.Email.Trim());
                if (isRepeatEmail)
                {
                    throw new LinCmsException("注册邮箱重复，请重新输入", ErrorCode.RepeatField);
                }
            }

            user.Active = UserActive.Active;
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
                new LinUserIdentity(LinUserIdentity.Password,user.Username,EncryptUtil.Encrypt(password),DateTime.Now)
            };
            await _userRepository.InsertAsync(user);
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
            LinUser linUser = await _userRepository.Where(r => r.Id == id).ToOneAsync();
            if (linUser == null)
            {
                throw new LinCmsException("用户不存在", ErrorCode.NotFound);
            }

            List<long> existGroupIds = await _groupService.GetGroupIdsByUserIdAsync(id);

            //删除existGroupIds有，而newGroupIds没有的
            List<long> deleteIds = existGroupIds.Where(r => !updateUserDto.GroupIds.Contains(r)).ToList();

            //添加newGroupIds有，而existGroupIds没有的
            List<long> addIds = updateUserDto.GroupIds.Where(r => !existGroupIds.Contains(r)).ToList();

            _mapper.Map(updateUserDto, linUser);
            await _userRepository.UpdateAsync(linUser);
            await _groupService.DeleteUserGroupAsync(id, deleteIds);
            await _groupService.AddUserGroupAsync(id, addIds);
        }

        public async Task ChangeStatusAsync(long id, UserActive userActive)
        {
            LinUser user = await _userRepository.Select.Where(r => r.Id == id).ToOneAsync();

            if (user == null)
            {
                throw new LinCmsException("用户不存在", ErrorCode.NotFound);
            }

            if (user.IsActive() && userActive == UserActive.Active)
            {
                throw new LinCmsException("当前用户已处于禁止状态");
            }

            if (!user.IsActive() && userActive == UserActive.NotActive)
            {
                throw new LinCmsException("当前用户已处于激活状态");
            }

            await _userRepository.UpdateDiy.Where(r => r.Id == id)
                                           .Set(r => new { Active = userActive.GetHashCode() })
                                           .ExecuteUpdatedAsync();
        }

        public async Task<LinUser> GetCurrentUserAsync()
        {
            if (_currentUser.Id != null)
            {
                long userId = (long)_currentUser.Id;
                return await _userRepository.Select.Where(r => r.Id == userId).ToOneAsync();
            }

            return null;
        }

        public async Task<UserInformation> GetInformationAsync(long userId)
        {
            LinUser linUser = await _userRepository.GetUserAsync(r => r.Id == userId);
            if (linUser == null) return null;
            linUser.Avatar = _fileRepository.GetFileUrl(linUser.Avatar);

            UserInformation userInformation = _mapper.Map<UserInformation>(linUser);
            userInformation.Groups = linUser.LinGroups.Select(r => _mapper.Map<GroupDto>(r)).ToList();
            userInformation.Admin = _currentUser.IsInGroup(LinConsts.Group.Admin);

            return userInformation;
        }

        public async Task<List<IDictionary<string, object>>> GetStructualUserPermissions(long userId)
        {
            List<LinPermission> permissions = await GetUserPermissions(userId);
            return _permissionService.StructuringPermissions(permissions);
        }

        /// <summary>
        /// 查找用户搜索分组，查找分组下的所有权限
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<LinPermission>> GetUserPermissions(long userId)
        {
            LinUser linUser = await _userRepository.GetUserAsync(r => r.Id == userId);
            List<long> groupIds = linUser.LinGroups.Select(r => r.Id).ToList();
            if (linUser.LinGroups == null || linUser.LinGroups.Count == 0)
            {
                return new List<LinPermission>();
            }

            return await _permissionService.GetPermissionByGroupIds(groupIds);
        }

    }
}