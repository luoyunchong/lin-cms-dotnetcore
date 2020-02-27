using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Core.Data;
using LinCms.Core.Dependency;
using LinCms.Core.Entities;
using LinCms.Infrastructure.Repositories;

namespace LinCms.Web.Data
{
    public interface IDataSeedContributor
    {
        Task SeedAsync();

    }
    public class DataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly AuditBaseRepository<LinPermission> _permissionRepository;
        public DataSeedContributor(AuditBaseRepository<LinPermission> permissionRepository)
        {
            this._permissionRepository =permissionRepository;
        }

        public Task SeedAsync()
        {
            List<PermissionDefinition> linCmsAttributes = ReflexHelper.GeAssemblyLinCmsAttributes();
            linCmsAttributes.ForEach(async r =>
            {
                bool exist = await _permissionRepository.Select
                    .Where(u => u.Module == r.Module && u.Name == r.Permission)
                    .AnyAsync();

                if (!exist)
                {
                    await _permissionRepository.InsertAsync(new LinPermission(r.Permission, r.Module));
                }

            });
            _permissionRepository.UnitOfWork.Commit();

            return Task.CompletedTask;
        }
    }
}
