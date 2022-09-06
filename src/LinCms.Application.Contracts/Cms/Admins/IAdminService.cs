using System.Collections.Generic;
using LinCms.Cms.Permissions;

namespace LinCms.Cms.Admins;

public interface IAdminService
{
    IDictionary<string, List<PermissionDto>> GetAllStructualPermissions();
}