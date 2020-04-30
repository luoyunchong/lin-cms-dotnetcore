using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Core.Data;
using LinCms.Core.Dependency;
using LinCms.Core.Entities;
using LinCms.Core.IRepositories;
using LinCms.Web.Middleware;
using LinCms.Web.Utils;

namespace LinCms.Web.Data
{
    public interface IDataSeedContributor
    {
        Task SeedAsync();

    }
    public class DataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private readonly IAuditBaseRepository<LinPermission> _permissionRepository;
        public DataSeedContributor(IAuditBaseRepository<LinPermission> permissionRepository)
        {
            _permissionRepository =permissionRepository;
        }

        [Transactional]
        public async Task SeedAsync()
        {
            List<PermissionDefinition> linCmsAttributes = ReflexHelper.GeAssemblyLinCmsAttributes();

            List<LinPermission> insertPermissions = new List<LinPermission>();
            List<LinPermission>allPermissions=await  _permissionRepository.Select.ToListAsync();
            
            linCmsAttributes.ForEach(r =>
            {
                bool exist = allPermissions.Any(u => u.Module == r.Module && u.Name == r.Permission);
                if (!exist)
                {
                    insertPermissions.Add(new LinPermission(r.Permission, r.Module));
                }
            });
            await _permissionRepository.InsertAsync(insertPermissions);
        }
    }
}
