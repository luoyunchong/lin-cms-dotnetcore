using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Entities.Settings;

namespace LinCms.IRepositories
{
    public interface ISettingRepository : IAuditBaseRepository<LinSetting>
    {
        Task<LinSetting> FindAsync(string name, string providerName, string providerKey);

        Task<List<LinSetting>> GetListAsync(string providerName, string providerKey);
    }
}
