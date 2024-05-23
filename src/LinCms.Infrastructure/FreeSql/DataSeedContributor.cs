using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.PortableExecutable;
using System.Threading;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Common;
using LinCms.Data;
using LinCms.Entities;
using Microsoft.Extensions.Logging;

namespace LinCms.FreeSql;

public class DataSeedContributor : IDataSeedContributor
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

        List<LinPermission> allPermissions = await _permissionRepository.Select.Where(r => r.PermissionType == PermissionType.Permission).ToListAsync(cancellationToken);

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


        #region Module 目录
        var allModules = await _permissionRepository.Select.Where(r => r.PermissionType == PermissionType.Folder).ToListAsync(cancellationToken);

        var permissionDefinitionsByModules = linCmsAttributes.GroupBy(r => r.Module).ToList();

        var insertMoudles = new List<LinPermission>();
        foreach (var module in permissionDefinitionsByModules)
        {
            LinPermission permissionEntity = allModules.FirstOrDefault(u => u.Name == module.Key);
            if (permissionEntity == null)
            {
                insertMoudles.Add(new LinPermission()
                {
                    PermissionType = PermissionType.Folder,
                    Name = module.Key,
                    ParentId = 0
                });
            }
        }
        await _permissionRepository.InsertAsync(insertMoudles, cancellationToken);
        #endregion

        allModules = await _permissionRepository.Select.Where(r => r.PermissionType == PermissionType.Folder).ToListAsync(cancellationToken);

        linCmsAttributes.ForEach(r =>
        {
            LinPermission permissionEntity = allPermissions.FirstOrDefault(u => u.Name == r.Permission);

            var parentId = allModules.Where(u => u.Name == r.Module).First().Id;
            if (permissionEntity == null)
            {
                insertPermissions.Add(new LinPermission(r.Permission, PermissionType.Permission, r.Router)
                {
                    ParentId = parentId
                });
            }
            else
            {
                bool routerExist = allPermissions.Any(u => u.Name == r.Permission && u.Router == r.Router);
                if (!routerExist)
                {
                    permissionEntity.Router = r.Router;
                    permissionEntity.ParentId = parentId;
                    permissionEntity.PermissionType  = PermissionType.Permission;
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