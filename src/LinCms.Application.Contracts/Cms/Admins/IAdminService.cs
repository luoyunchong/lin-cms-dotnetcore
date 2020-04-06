using System.Collections.Generic;
using LinCms.Application.Contracts.Cms.Permissions.Dtos;

namespace LinCms.Application.Contracts.Cms.Admins
{
    public interface IAdminService
    {
        IDictionary<string, List<PermissionDto>> GetAllStructualPermissions();
    }
}
