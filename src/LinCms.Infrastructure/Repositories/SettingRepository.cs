using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FreeSql;
using IGeekFan.FreeKit.Extras.FreeSql;
using IGeekFan.FreeKit.Extras.Security;
using LinCms.Entities.Settings;
using LinCms.IRepositories;

namespace LinCms.Repositories;

public class SettingRepository : AuditDefaultRepository<LinSetting,Guid,long>, ISettingRepository
{
    public SettingRepository(UnitOfWorkManager unitOfWorkManager, ICurrentUser currentUser) : base(unitOfWorkManager, currentUser)
    {
    }

    public async Task<LinSetting> FindAsync(string name, string providerName, string providerKey)
    {
        return await Select.Where(s => s.Name == name && s.ProviderName == providerName && s.ProviderKey == providerKey)
            .FirstAsync();
    }

    public async Task<List<LinSetting>> GetListAsync(string providerName, string providerKey)
    {
        return await Select
            .Where(
                s => s.ProviderName == providerName && s.ProviderKey == providerKey
            ).ToListAsync();
    }
}