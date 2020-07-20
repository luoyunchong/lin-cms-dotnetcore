using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LinCms.Cms.Admins;
using LinCms.Cms.Permissions;
using LinCms.Entities;
using LinCms.IRepositories;

namespace LinCms.Cms.Admin
{
    public class AdminService : IAdminService
    {
        private readonly IAuditBaseRepository<LinPermission> _permissionRepository;
        private readonly IMapper _mapper;
        public AdminService(IAuditBaseRepository<LinPermission> permissionRepository, IMapper mapper)
        {
            _permissionRepository = permissionRepository;
            _mapper = mapper;
        }

        public IDictionary<string, List<PermissionDto>> GetAllStructualPermissions()
        {
            return _permissionRepository.Select.ToList()
                 .GroupBy(r => r.Module)
                 .ToDictionary(
                     group => group.Key,
                     group =>
                         _mapper.Map<List<PermissionDto>>(group.ToList())
                   );

        }
    }
}
