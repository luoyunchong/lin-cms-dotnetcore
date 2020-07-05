using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using LinCms.Core.Common;
using LinCms.Core.Data;
using LinCms.Core.Dependency;
using LinCms.Core.Entities;
using LinCms.Core.IRepositories;
using LinCms.Web.Middleware;
using LinCms.Web.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LinCms.Web.Data
{
    public interface IDataSeedContributor
    {
        Task SeedPermissionAsync();

        Task InitAdminPermission();

    }
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
        public async Task SeedPermissionAsync()
        {
            List<PermissionDefinition> linCmsAttributes = ReflexHelper.GeAssemblyLinCmsAttributes();

            List<LinPermission> insertPermissions = new List<LinPermission>();
            List<LinPermission> allPermissions = await _permissionRepository.Select.ToListAsync();

            Expression<Func<LinGroupPermission, bool>> expression = u => false;
            Expression<Func<LinPermission, bool>> permissionExpression = u => false;
            allPermissions.ForEach(permissioin =>
            {
                if (!linCmsAttributes.Any(r => r.Permission == permissioin.Name))
                {
                    expression = expression.Or(r => r.PermissionId == permissioin.Id);
                    permissionExpression = permissionExpression.Or(r => r.Id == permissioin.Id);
                }
            });

            int effectRows = await _permissionRepository.DeleteAsync(permissionExpression);
            effectRows += await _groupPermissionRepository.DeleteAsync(expression);
            _logger.LogInformation($"删除了{effectRows}条数据");

            linCmsAttributes.ForEach(r =>
            {
                bool exist = allPermissions.Any(u => u.Module == r.Module && u.Name == r.Permission);
                if (!exist)
                {
                    insertPermissions.Add(new LinPermission(r.Permission, r.Module));
                }
            });
            await _permissionRepository.InsertAsync(insertPermissions);
            _logger.LogInformation($"新增了{insertPermissions.Count}条数据");
        }
    }
}
