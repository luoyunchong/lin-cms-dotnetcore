using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using LinCms.Common;
using LinCms.Data;
using LinCms.Dependency;
using LinCms.Entities;
using LinCms.IRepositories;
using Microsoft.Extensions.Logging;

namespace LinCms.FreeSql
{
    public class DataSeedContributor : IDataSeedContributor, ISingletonDependency
    {
        private readonly IAuditBaseRepository<LinPermission> _permissionRepository;
        private readonly IAuditBaseRepository<LinGroupPermission> _groupPermissionRepository;
        private readonly ILogger<DataSeedContributor> _logger;
        public DataSeedContributor(IAuditBaseRepository<LinPermission> permissionRepository, IAuditBaseRepository<LinGroupPermission> groupPermissionRepository, ILogger<DataSeedContributor> logger)
        {
            _permissionRepository = permissionRepository;
            _groupPermissionRepository = groupPermissionRepository;
            _logger = logger;
        }

        public async Task InitAdminPermission()
        {
            bool valid = await _groupPermissionRepository.Select.AnyAsync();
            if (valid) return;

            List<LinPermission> allPermissions = await _permissionRepository.Select.ToListAsync();

            List<LinGroupPermission> groupPermissions = allPermissions.Select(u => new LinGroupPermission(LinConsts.Group.Admin, u.Id)).ToList();

            await _groupPermissionRepository.InsertAsync(groupPermissions);

        }

        /// <summary>
        /// 权限标签上的Permission改变时，删除数据库中存在的无效权限，并生成新的权限。
        /// </summary>
        /// <returns></returns>
        public async Task SeedPermissionAsync(List<PermissionDefinition> linCmsAttributes, CancellationToken cancellationToken)
        {

            List<LinPermission> insertPermissions = new();
            List<LinPermission> updatePermissions = new();

            List<LinPermission> allPermissions = await _permissionRepository.Select.ToListAsync();

            Expression<Func<LinGroupPermission, bool>> expression = u => false;
            Expression<Func<LinPermission, bool>> permissionExpression = u => false;

            allPermissions.ForEach(permissioin =>
            {
                if (linCmsAttributes.All(r => r.Permission != permissioin.Name))
                {
                    expression = expression.Or(r => r.PermissionId == permissioin.Id);
                    permissionExpression = permissionExpression.Or(r => r.Id == permissioin.Id);
                }
            });
            int effectRows = await _permissionRepository.DeleteAsync(permissionExpression, cancellationToken);
            effectRows += await _groupPermissionRepository.DeleteAsync(expression, cancellationToken);
            _logger.LogInformation($"删除了{effectRows}条数据");

            linCmsAttributes.ForEach(r =>
            {
                LinPermission permissionEntity = allPermissions.FirstOrDefault(u => u.Module == r.Module && u.Name == r.Permission);
                if (permissionEntity == null)
                {
                    insertPermissions.Add(new LinPermission(r.Permission, r.Module, r.Router));
                }
                else
                {
                    bool routerExist = allPermissions.Any(u => u.Module == r.Module && u.Name == r.Permission && u.Router == r.Router);
                    if (!routerExist)
                    {
                        permissionEntity.Router = r.Router;
                        updatePermissions.Add(permissionEntity);
                    }
                }
            });

            await _permissionRepository.InsertAsync(insertPermissions, cancellationToken);
            _logger.LogInformation($"新增了{insertPermissions.Count}条数据");

            await _permissionRepository.UpdateAsync(updatePermissions, cancellationToken);
            _logger.LogInformation($"更新了{updatePermissions.Count}条数据");
        }
    }
}
