using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FreeSql;
using LinCms.Core.Entities.Settings;
using LinCms.Core.IRepositories;
using LinCms.Core.Security;

namespace LinCms.Infrastructure.Repositories
{
    public class SettingRepository : AuditBaseRepository<LinSetting>, ISettingRepository
    {
        public SettingRepository(UnitOfWorkManager unitOfWorkManager, ICurrentUser currentUser): base(unitOfWorkManager, currentUser)
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
}
