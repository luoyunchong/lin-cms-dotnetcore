using AutoMapper;
using FreeSql;
using LinCms.Web.Models.Admins;
using LinCms.Web.Models.Users;
using LinCms.Web.Services.Interfaces;
using LinCms.Zero.Data;
using LinCms.Zero.Domain;
using LinCms.Zero.Extensions;

namespace LinCms.Web.Services
{
    public class UserService : IUserSevice
    {
        private readonly BaseRepository<LinUser> _userRepository;
        private readonly IFreeSql _freeSql;
        private readonly IMapper _mapper;

        public UserService(BaseRepository<LinUser> userRepository, IFreeSql freeSql, IMapper mapper)
        {
            _userRepository = userRepository;
            _freeSql = freeSql;
            _mapper = mapper;
        }

        public LinUser Authorization(string username, string password)
        {
            LinUser user = _userRepository.Select.Where(r => r.Nickname == username && r.Password == password).First();

            return user;
        }

        public bool ChangePassword(ChangePasswordDto passwordDto)
        {
            throw new System.NotImplementedException();
        }

        public ResultDto Delete(int id)
        {
            _userRepository.Delete(r => r.Id == id);
            return ResultDto.Success();
        }

        public PagedResultDto<LinUser> GetUserList(UserSearchDto searchDto)
        {
            var linUsers = _userRepository.Select.WhereIf(searchDto.GroupId != null, r => r.GroupId == searchDto.GroupId)
                  .ToPagerList(searchDto, out long totalNums);

            return new PagedResultDto<LinUser>(linUsers, totalNums);
        }

        public ResultDto Register(LinUser user)
        {
            var isRepeatNickName = _userRepository.Where(r => r.Nickname == user.Nickname).Any();

            if (isRepeatNickName)
            {
                return ResultDto.Error("用户名重复，请重新输入");
            }

            if (!string.IsNullOrEmpty(user.Email.Trim()))
            {
                var isRepeatEmail= _userRepository.Where(r => r.Email == user.Email.Trim()).Any();
                if (isRepeatEmail)
                {
                    return ResultDto.Error("注册邮箱重复，请重新输入");
                }
            }

            user.Active = 1;
            user.Admin = 1;

            _userRepository.Insert(user);
            return ResultDto.Success("用户创建成功");
        }

        /// <summary>
        /// 修改指定字段，邮件和组别
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateUserDto"></param>    
        /// <returns></returns>
        public ResultDto UpdateUserInfo(int id, UpdateUserDto updateUserDto)
        {
            //此方法适用于更新字段少时
            //_freeSql.Update<LinUser>(id).Set(a => new LinUser()
            //{
            //    Email = updateUserDto.Email,
            //    GroupId = updateUserDto.GroupId
            //}).ExecuteAffrows();

            //需要多查一次
            LinUser linUser = _userRepository.Where(r => r.Id == id).ToOne();

            //赋值过程可使用AutoMapper简化
            //只更新 Email、GroupId
            // UPDATE `lin_user` SET `email` = ?p_0, `group_id` = ?p_1 
            // WHERE(`id` = 1) AND(`is_deleted` = 0)
            //linUser.Email = updateUserDto.Email;
            //linUser.GroupId = updateUserDto.GroupId;

            linUser = _mapper.Map<LinUser>(updateUserDto);

            linUser.Id = id;

            _userRepository.Update(linUser);

            return ResultDto.Success("操作成功");
        }
    }
}
