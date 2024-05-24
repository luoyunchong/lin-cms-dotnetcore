using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Data;
using LinCms.Entities;

namespace LinCms.Cms.Permissions;

public interface IPermissionService
{
    Task<List<PermissionTreeNode>> GetPermissionTreeNodes();
    Task<bool> CheckPermissionAsync(string permission);
    Task DeletePermissionsAsync(RemovePermissionDto permissionDto);

    Task DispatchPermissions(DispatchPermissionsDto permissionDto, List<PermissionDefinition> permissionDefinition);

    Task<List<LinPermission>> GetPermissionByGroupIds(List<long> groupIds);

    List<IDictionary<string, object>> StructuringPermissions(List<LinPermission> permissions);

    Task UpdateAsync(int id,PermissioCreateUpdateDto createUpdateDto);
    Task CreateAsync(PermissioCreateUpdateDto createUpdateDto);

    Task DeleteAsync(int id);

    Task<LinPermission> GetAsync(string permissionName);

}