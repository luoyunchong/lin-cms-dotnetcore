using DotNetCore.Security;
using LinCms.Aop.Attributes;
using LinCms.Entities;
using LinCms.Exceptions;
using LinCms.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinCms.Cms.Users
{
    public class UserIdentityService : ApplicationService, IUserIdentityService
    {
        private readonly IAuditBaseRepository<LinUserIdentity> _userIdentityRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICryptographyService _cryptographyService;
        public UserIdentityService(IAuditBaseRepository<LinUserIdentity> userIdentityRepository, ICryptographyService cryptographyService, IUserRepository userRepository)
        {
            _userIdentityRepository = userIdentityRepository;
            _cryptographyService = cryptographyService;
            _userRepository = userRepository;
        }

        public async Task<bool> VerifyUserPasswordAsync(long userId, string password, string salt)
        {
            LinUserIdentity userIdentity = await this.GetFirstByUserIdAsync(userId);
            string encryptPassword = _cryptographyService.Encrypt(password, salt);
            return userIdentity != null && userIdentity.Credential == encryptPassword;
        }


        public async Task ChangePasswordAsync(long userId, string newpassword, string salt)
        {
            var linUserIdentity = await  this.GetFirstByUserIdAsync(userId); ;

            await this.ChangePasswordAsync(linUserIdentity, newpassword, salt);
        }


        public Task ChangePasswordAsync(LinUserIdentity linUserIdentity, string newpassword, string salt)
        {
            string encryptPassword = _cryptographyService.Encrypt(newpassword, salt);
            if (linUserIdentity == null)
            {
                linUserIdentity = new LinUserIdentity(LinUserIdentity.Password, "", encryptPassword, DateTime.Now);
                return _userIdentityRepository.InsertAsync(linUserIdentity);
            }
            else
            {
                linUserIdentity.Credential = encryptPassword;
                return _userIdentityRepository.UpdateAsync(linUserIdentity);
            }
        }

        [Transactional]
        public Task DeleteAsync(long userId)
        {
            return _userIdentityRepository.Where(r => r.CreateUserId == userId).ToDelete().ExecuteAffrowsAsync();
        }

        public Task<LinUserIdentity> GetFirstByUserIdAsync(long userId)
        {
            return _userIdentityRepository
                .Where(r => r.CreateUserId == userId && r.IdentityType == LinUserIdentity.Password)
                .FirstAsync();
        }

        public async Task<List<UserIdentityDto>> GetListAsync(long userId)
        {
            List<LinUserIdentity> userIdentities = await _userIdentityRepository
                .Where(r => r.CreateUserId == userId)
                .ToListAsync();

            return Mapper.Map<List<UserIdentityDto>>(userIdentities);
        }

        public async Task UnBind(Guid id)
        {
            LinUserIdentity userIdentity = await _userIdentityRepository.GetAsync(id);
            if (userIdentity == null || userIdentity.CreateUserId != CurrentUser.Id)
            {
                throw new LinCmsException("你无权解绑此账号");
            }

            List<LinUserIdentity> userIdentities = await _userIdentityRepository.Select.Where(r => r.CreateUserId == CurrentUser.Id).ToListAsync();

            bool hasPwd = userIdentities.Where(r => r.IdentityType == LinUserIdentity.Password).Any();

            if (!hasPwd && userIdentities.Count == 1)
            {
                throw new LinCmsException("你未设置密码，无法解绑最后一个第三方登录账号");
            }
            await _userIdentityRepository.DeleteAsync(userIdentity);
        }
    }
}
