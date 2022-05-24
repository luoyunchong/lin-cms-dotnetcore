using System.Collections.Generic;
using System.Linq;
using LinCms.Cms.Admins;
using LinCms.Cms.Permissions;
using LinCms.Entities;
using LinCms.IRepositories;

namespace LinCms.Cms.Admin
{
    public class AdminService : ApplicationService, IAdminService
    {
        private readonly IAuditBaseRepository<LinPermission> _permissionRepository;
        public AdminService(IAuditBaseRepository<LinPermission> permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public IDictionary<string, List<PermissionDto>> GetAllStructualPermissions()
        {
            return _permissionRepository.Select.ToList()
                 .GroupBy(r => r.Module)
                 .ToDictionary(
                     group => group.Key,
                     group =>
                         Mapper.Map<List<PermissionDto>>(group.ToList())
                   );

        }
    }
}
