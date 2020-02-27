using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Application.Contracts.Cms.Admins;
using LinCms.Application.Contracts.Cms.Groups;
using LinCms.Application.Contracts.Cms.Users;
using LinCms.Core.Common;
using LinCms.Core.Data;
using LinCms.Core.Data.Enums;
using LinCms.Core.Entities;
using LinCms.Core.Exceptions;
using LinCms.Core.Extensions;
using LinCms.Core.Security;
using LinCms.Infrastructure.Repositories;

namespace LinCms.Application.Cms.Users
{
    public class UserService : IUserService
    {
        private readonly UserRepository _userRepository;
        private readonly IFreeSql _freeSql;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        private readonly IUserIdentityService _userIdentityService;
        public UserService(UserRepository userRepository,
            IFreeSql freeSql,
            IMapper mapper,
            ICurrentUser currentUser,
            IUserIdentityService userIdentityService)
        {
            _userRepository = userRepository;
            _freeSql = freeSql;
            _mapper = mapper;
            _currentUser = currentUser;
            _userIdentityService = userIdentityService;
        }

        public Task<LinUser> GetUserAsync(Expression<Func<LinUser, bool>> expression)
        {
            return _userRepository.Select.Where(expression).IncludeMany(r => r.LinGroups).ToOneAsync();
        }

        public async Task ChangePasswordAsync(ChangePasswordDto passwordDto)
        {
            long currentUserId = _currentUser.Id ?? 0;

            bool valid = _userIdentityService.VerifyUsernamePassword(currentUserId, _currentUser.UserName,
                  passwordDto.OldPassword);
            if (valid)
            {
                throw new LinCmsException("旧密码不正确");
            }

            await _userIdentityService.ChangePasswordAsync(currentUserId, passwordDto.NewPassword);
        }


        public async Task DeleteAsync(int id)
        {
            await _userRepository.DeleteAsync(r => r.Id == id);
        }

        public async Task ResetPasswordAsync(int id, ResetPasswordDto resetPasswordDto)
        {
            bool userExist = await _userRepository.Where(r => r.Id == id).AnyAsync();

            if (userExist == false)
            {
                throw new LinCmsException("用户不存在", ErrorCode.NotFound);
            }

            string confirmPassword = EncryptUtil.Encrypt(resetPasswordDto.ConfirmPassword);

            await _freeSql.Update<LinUser>(id).Set(a => new LinUser()
            {
                Password = confirmPassword
            }).ExecuteAffrowsAsync();

        }

        public PagedResultDto<UserDto> GetUserListByGroupId(UserSearchDto searchDto)
        {
            List<UserDto> linUsers = _userRepository.Select
                .IncludeMany(r => r.LinGroups)
                .Where(r => r.Admin == (int)UserAdmin.Common)
                .WhereIf(searchDto.GroupId != null, r => r.LinGroups.Any(u => u.Id == searchDto.GroupId))
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

        public async Task Register(LinUser user)
        {
            if (!string.IsNullOrEmpty(user.Username))
            {
                bool isRepeatName = _userRepository.Select.Any(r => r.Username == user.Username);

                if (isRepeatName)
                {
                    throw new LinCmsException("用户名重复，请重新输入", ErrorCode.RepeatField);
                }
            }

            if (!string.IsNullOrEmpty(user.Email.Trim()))
            {
                var isRepeatEmail = _userRepository.Select.Any(r => r.Email == user.Email.Trim());
                if (isRepeatEmail)
                {
                    throw new LinCmsException("注册邮箱重复，请重新输入", ErrorCode.RepeatField);
                }
            }

            user.Active = UserActive.Active.GetHashCode();
            user.Admin = UserAdmin.Common.GetHashCode();
            user.LinUserIdentitys = new List<LinUserIdentity>()
            {
                new LinUserIdentity()
                {
                    IdentityType = LinUserIdentity.Password,
                    Credential = EncryptUtil.Encrypt(user.Password),
                    Identifier = user.Username
                }
            };
            await _userRepository.InsertAsync(user);

        }

        /// <summary>
        /// 修改指定字段，邮件和组别
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateUserDto"></param>    
        /// <returns></returns>
        public void UpdateUserInfo(int id, UpdateUserDto updateUserDto)
        {
            //此方法适用于更新字段少时
            //_freeSql.Update<LinUser>(id).Set(a => new LinUser()
            //{
            //    Email = updateUserDto.Email,
            //    GroupId = updateUserDto.GroupId
            //}).ExecuteAffrows();

            //需要多查一次
            LinUser linUser = _userRepository.Where(r => r.Id == id).ToOne();
            if (linUser == null)
            {
                throw new LinCmsException("用户不存在", ErrorCode.NotFound);
            }
            //赋值过程可使用AutoMapper简化
            //只更新 Email、GroupId
            // UPDATE `lin_user` SET `email` = ?p_0, `group_id` = ?p_1 
            // WHERE(`id` = 1) AND(`is_deleted` = 0)
            //linUser.Email = updateUserDto.Email;
            //linUser.GroupId = updateUserDto.GroupId;

            _mapper.Map(updateUserDto, linUser);

            _userRepository.Update(linUser);

        }


        public async Task ChangeStatusAsync(int id, UserActive userActive)
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

            await _freeSql.Update<LinUser>(id).Set(a => new LinUser()
            {
                Active = userActive.GetHashCode()
            }).ExecuteAffrowsAsync();
        }

        public bool CheckPermission(int userId, string permission)
        {
            long[] groups = _currentUser.Groups;

            bool existPermission = _freeSql.Select<LinGroupPermission>().Any(r => groups.Contains(r.GroupId)); //&& r. == permission);
                //TODO
            return existPermission;
        }

        public LinUser GetCurrentUser()
        {
            if (_currentUser.Id != null)
            {
                long userId = (long)_currentUser.Id;
                return _userRepository.Select.Where(r => r.Id == userId).ToOne();
            }
            return null;
        }
    }
}
