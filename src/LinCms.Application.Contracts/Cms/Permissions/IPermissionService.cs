using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Data;
using LinCms.Entities;

namespace LinCms.Cms.Permissions;

public interface IPermissionService
{
    Task<List<PermissionTreeNode>> GetPermissionTreeNodes();
    Task<bool> CheckPermissionAsync(string module, string permission);
    Task DeletePermissionsAsync(RemovePermissionDto permissionDto);

    Task DispatchPermissions(DispatchPermissionsDto permissionDto, List<PermissionDefinition> permissionDefinition);

    Task<List<LinPermission>> GetPermissionByGroupIds(List<long> groupIds);

    List<IDictionary<string, object>> StructuringPermissions(List<LinPermission> permissions);

    Task<LinPermission> GetAsync(string permissionName);

}