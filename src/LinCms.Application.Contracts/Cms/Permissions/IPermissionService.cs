using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Application.Contracts.Cms.Permissions.Dtos;
using LinCms.Core.Data;
using LinCms.Core.Entities;

namespace LinCms.Application.Contracts.Cms.Permissions
{
    public interface IPermissionService
    {
        Task<bool> CheckPermissionAsync( string permission);
        Task DeletePermissionsAsync(RemovePermissionDto permissionDto);

        Task DispatchPermissions(DispatchPermissionsDto permissionDto, List<PermissionDefinition> permissionDefinition);

        Task<List<LinPermission>> GetPermissionByGroupIds(List<long> groupIds);

        List<IDictionary<string, object>> StructuringPermissions(List<LinPermission> permissions);


    }
}
