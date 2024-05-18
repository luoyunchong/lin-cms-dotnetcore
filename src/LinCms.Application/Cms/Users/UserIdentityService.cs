using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.Security;
using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Entities;
using LinCms.Exceptions;
using LinCms.Security;

namespace LinCms.Cms.Users;

public class UserIdentityService(IAuditBaseRepository<LinUserIdentity> userIdentityRepository,
        ICryptographyService cryptographyService)
    : ApplicationService, IUserIdentityService
{
    public async Task<bool> VerifyUserPasswordAsync(long userId, string password, string salt)
    {
        LinUserIdentity userIdentity = await GetFirstByUserIdAsync(userId);
        //快速登录时，用户实际未设置密码
        if (userIdentity == null)
        {
            return true;
        }
        string encryptPassword = cryptographyService.Encrypt(password, salt);
        return userIdentity.Credential == encryptPassword;
    }


    public async Task ChangePasswordAsync(long userId, string newpassword, string salt)
    {
        var linUserIdentity = await GetFirstByUserIdAsync(userId); ;

        await ChangePasswordAsync(linUserIdentity, newpassword, salt);
    }


    public Task ChangePasswordAsync(LinUserIdentity linUserIdentity, string newpassword, string salt)
    {
        string encryptPassword = cryptographyService.Encrypt(newpassword, salt);
        if (linUserIdentity == null)
        {
            linUserIdentity = new LinUserIdentity(LinUserIdentity.Password, "", encryptPassword, DateTime.Now);
            return userIdentityRepository.InsertAsync(linUserIdentity);
        }
        else
        {
            linUserIdentity.Credential = encryptPassword;
            return userIdentityRepository.UpdateAsync(linUserIdentity);
        }
    }

    [Transactional]
    public Task DeleteAsync(long userId)
    {
        return userIdentityRepository.Where(r => r.CreateUserId == userId).ToDelete().ExecuteAffrowsAsync();
    }

    public Task<LinUserIdentity> GetFirstByUserIdAsync(long userId)
    {
        return userIdentityRepository
            .Where(r => r.CreateUserId == userId && r.IdentityType == LinUserIdentity.Password)
            .FirstAsync();
    }

    public async Task<List<UserIdentityDto>> GetListAsync(long userId)
    {
        List<LinUserIdentity> userIdentities = await userIdentityRepository
            .Where(r => r.CreateUserId == userId)
            .ToListAsync();

        return Mapper.Map<List<UserIdentityDto>>(userIdentities);
    }

    public async Task UnBind(Guid id)
    {
        LinUserIdentity userIdentity = await userIdentityRepository.GetAsync(id);
        if (userIdentity == null || userIdentity.CreateUserId !=  CurrentUser.FindUserId())
        {
            throw new LinCmsException("你无权解绑此账号");
        }

        List<LinUserIdentity> userIdentities = await userIdentityRepository.Select.Where(r => r.CreateUserId ==  CurrentUser.FindUserId()).ToListAsync();

        bool hasPwd = userIdentities.Any(r => r.IdentityType == LinUserIdentity.Password);

        if (!hasPwd && userIdentities.Count == 1)
        {
            throw new LinCmsException("你未设置密码，无法解绑最后一个第三方登录账号");
        }
        await userIdentityRepository.DeleteAsync(userIdentity);
    }
}