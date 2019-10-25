using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FreeSql;
using LinCms.Web.Models.Cms.Admins;
using LinCms.Web.Models.Cms.Users;
using LinCms.Web.Services.Interfaces;
using LinCms.Zero.Common;
using LinCms.Zero.Data;
using LinCms.Zero.Data.Enums;
using LinCms.Zero.Domain;
using LinCms.Zero.Exceptions;
using LinCms.Zero.Repositories;
using LinCms.Zero.Security;

namespace LinCms.Web.Services
{
    public class UserService : IUserSevice
    {
        private readonly AuditBaseRepository<LinUser> _userRepository;
        private readonly BaseRepository<LinGroup> _groupRepository;
        private readonly IFreeSql _freeSql;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        public UserService(AuditBaseRepository<LinUser> userRepository, IFreeSql freeSql, IMapper mapper, ICurrentUser currentUser, BaseRepository<LinGroup> groupRepository)
        {
            _userRepository = userRepository;
            _freeSql = freeSql;
            _mapper = mapper;
            _currentUser = currentUser;
            _groupRepository = groupRepository;
        }

        public LinUser Authorization(string username, string password)
        {
            LinUser user = _userRepository.Select.Where(r => r.Username == username && r.Password == LinCmsUtils.Get32Md5(password)).ToOne();

            return user;
        }

        public void ChangePassword(ChangePasswordDto passwordDto)
        {
            string oldPassword = LinCmsUtils.Get32Md5(passwordDto.OldPassword);

            _userRepository.Select.Any(r => r.Password == oldPassword && r.Id == _currentUser.Id);

            string newPassword = LinCmsUtils.Get32Md5(passwordDto.NewPassword);

            _freeSql.Update<LinUser>(_currentUser.Id).Set(a => new LinUser()
            {
                Password = newPassword
            }).ExecuteAffrows();
        }

        public void Delete(int id)
        {
            _userRepository.Delete(r => r.Id == id);
        }

        public void ResetPassword(int id, ResetPasswordDto resetPasswordDto)
        {
            bool userExist = _userRepository.Where(r => r.Id == id).Any();

            if (userExist == false)
            {
                throw new LinCmsException("用户不存在", ErrorCode.NotFound);
            }

            string confirmPassword = LinCmsUtils.Get32Md5(resetPasswordDto.ConfirmPassword);

            _freeSql.Update<LinUser>(id).Set(a => new LinUser()
            {
                Password = confirmPassword
            }).ExecuteAffrows();

        }

        /// <summary>
        /// https://github.com/2881099/FreeSql/wiki/%e8%bf%94%e5%9b%9e%e6%9f%a5%e8%af%a2%e7%9a%84%e6%95%b0%e6%8d%ae   返回更为复杂的结构
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public PagedResultDto<UserDto> GetUserList(UserSearchDto searchDto)
        {
            ISelect<LinUser> select = _userRepository.Select
                .Where(r => r.Admin == (int)UserAdmin.Common)
                .WhereIf(searchDto.GroupId != null, r => r.GroupId == searchDto.GroupId);

            List<UserDto> linUsers = select
                .OrderByDescending(r => r.Id)
                .From<LinGroup>((a, b) =>
                            a.LeftJoin(c => c.GroupId == b.Id)
                )
                .Page(searchDto.Page+1, searchDto.Count)
                .ToList((a, b) => new
                {
                    user = a,
                    GroupName = b.Name
                }).Select(r =>
                {
                    UserDto userDto = _mapper.Map<UserDto>(r.user);
                    userDto.GroupName = r.GroupName;
                    return userDto;
                }).ToList();

            long totalNums = select.Count();

            return new PagedResultDto<UserDto>(linUsers, totalNums);
        }

        public void Register(LinUser user)
        {
            bool isExistGroup = _groupRepository.Select.Any(r => r.Id == user.GroupId);

            if (!isExistGroup)
            {
                throw new LinCmsException("分组不存在", ErrorCode.NotFound);
            }

            bool isRepeatName = _userRepository.Select.Any(r => r.Username == user.Username);

            if (isRepeatName)
            {
                throw new LinCmsException("用户名重复，请重新输入", ErrorCode.RepeatField);
            }

            if (!string.IsNullOrEmpty(user.Email.Trim()))
            {
                var isRepeatEmail = _userRepository.Select.Any(r => r.Email == user.Email.Trim());
                if (isRepeatEmail)
                {
                    throw new LinCmsException("注册邮箱重复，请重新输入", ErrorCode.RepeatField);
                }
            }

            user.Active = 1;
            user.Admin = 1;
            user.Password = LinCmsUtils.Get32Md5(user.Password);

            _userRepository.Insert(user);
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


        public void ChangeStatus(int id, UserActive userActive)
        {
            LinUser user = _userRepository.Select.Where(r => r.Id == id).ToOne();

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

            _freeSql.Update<LinUser>(id).Set(a => new LinUser()
            {
                Active = userActive.GetHashCode()
            }).ExecuteAffrows();
        }

        public bool CheckPermission(int userId, string permission)
        {
            int? groupId = _currentUser.GroupId;

            if (groupId == 0 || groupId == null)
            {
                throw new LinCmsException("当前用户无任何分组！");
            }

            bool existPermission = _freeSql.Select<LinAuth>().Any(r => r.GroupId == groupId && r.Auth == permission);

            return existPermission;
        }

        public LinUser GetCurrentUser()
        {
            if (_currentUser.Id != null)
            {
                int userId = (int) _currentUser.Id;
                return _userRepository.Select.Where(r => r.Id == userId).ToOne();
            }
            return null;
        }
    }
}
