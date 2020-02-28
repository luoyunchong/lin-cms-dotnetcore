using System.Collections.Generic;
using LinCms.Application.Contracts.Cms.Permissions;

namespace LinCms.Application.Cms.Admin
{
    public interface IAdminService
    {
        IDictionary<string, List<PermissionDto>> GetAllStructualPermissions();
    }
}
