using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Core.Entities.Settings;

namespace LinCms.Core.IRepositories
{
    public interface ISettingRepository : IAuditBaseRepository<LinSetting>
    {
        Task<LinSetting> FindAsync(string name, string providerName, string providerKey);

        Task<List<LinSetting>> GetListAsync(string providerName, string providerKey);
    }
}
