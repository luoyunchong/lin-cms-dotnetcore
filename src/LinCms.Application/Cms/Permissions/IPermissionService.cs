using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LinCms.Application.Contracts.Cms.Permissions;
using LinCms.Core.Data;

namespace LinCms.Application.Cms.Permissions
{
    public interface IPermissionService
    {
        Task RemovePermissions(PermissionDto permissionDto);

        Task DispatchPermissions(PermissionDto permissionDto, List<PermissionDefinition> permissionDefinition);
    }
}
