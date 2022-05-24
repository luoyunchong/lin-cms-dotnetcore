using System.Collections.Generic;
using System.Threading.Tasks;
using FreeSql;
using LinCms.Entities.Settings;
using LinCms.IRepositories;
using LinCms.Security;

namespace LinCms.Repositories
{
    public class SettingRepository : AuditBaseRepository<LinSetting>, ISettingRepository
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
}
