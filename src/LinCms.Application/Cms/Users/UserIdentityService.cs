using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LinCms.Aop.Attributes;
using LinCms.Common;
using LinCms.Data;
using LinCms.Entities;
using LinCms.Exceptions;
using LinCms.IRepositories;

namespace LinCms.Cms.Users
{
    public class UserIdentityService : ApplicationService, IUserIdentityService
    {
        private readonly IAuditBaseRepository<LinUserIdentity> _userIdentityRepository;

        public UserIdentityService(IAuditBaseRepository<LinUserIdentity> userIdentityRepository)
        {
            _userIdentityRepository = userIdentityRepository;
        }

        public async Task<bool> VerifyUserPasswordAsync(long userId, string password)
        {
            LinUserIdentity userIdentity = await this.GetFirstByUserIdAsync(userId);

            return userIdentity != null && EncryptUtil.Verify(userIdentity.Credential, password);
        }


        public async Task ChangePasswordAsync(long userId, string newpassword)
        {
            var linUserIdentity = await _userIdentityRepository.Where(a => a.CreateUserId == userId && a.IdentityType == LinUserIdentity.Password).FirstAsync();
            await this.ChangePasswordAsync(linUserIdentity, newpassword);
        }


        public async Task ChangePasswordAsync(LinUserIdentity linUserIdentity, string newpassword)
        {
            string encryptPassword = EncryptUtil.Encrypt(newpassword);
            if (linUserIdentity == null)
            {
                linUserIdentity = new LinUserIdentity(LinUserIdentity.Password, "", encryptPassword, DateTime.Now);
                await _userIdentityRepository.InsertAsync(linUserIdentity);
            }
            else
            {
                linUserIdentity.Credential = encryptPassword;
                await _userIdentityRepository.UpdateAsync(linUserIdentity);
            }
        }

        [Transactional]
        public async Task DeleteAsync(long userId)
        {
            await _userIdentityRepository.Where(r => r.CreateUserId == userId).ToDelete().ExecuteAffrowsAsync();
        }

        public async Task<LinUserIdentity> GetFirstByUserIdAsync(long userId)
        {
            return await _userIdentityRepository
                .Where(r => r.CreateUserId == userId && r.IdentityType == LinUserIdentity.Password)
                .ToOneAsync();
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