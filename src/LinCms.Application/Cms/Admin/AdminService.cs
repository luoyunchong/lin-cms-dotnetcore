using System.Collections.Generic;
using System.Linq;
using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Cms.Admins;
using LinCms.Cms.Permissions;
using LinCms.Entities;

namespace LinCms.Cms.Admin;

public class AdminService(IAuditBaseRepository<LinPermission> permissionRepository) : ApplicationService, IAdminService
{
    public IDictionary<string, List<PermissionDto>> GetAllStructualPermissions()
    {
        return permissionRepository.Select.ToList()
            .GroupBy(r => r.Module)
            .ToDictionary(
                group => group.Key,
                group =>
                    Mapper.Map<List<PermissionDto>>(group.ToList())
            );

    }
}