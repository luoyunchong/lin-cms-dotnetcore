using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Application.Contracts.Cms.Permissions;
using LinCms.Application.Contracts.Cms.Permissions.Dtos;
using LinCms.Core.Data;
using LinCms.Core.Entities;
using LinCms.Core.Security;

namespace LinCms.Application.Cms.Permissions
{
    public class PermissionService : IPermissionService
    {
        private readonly IFreeSql _freeSql;
        private readonly ICurrentUser _currentUser;
        public PermissionService(IFreeSql freeSql, ICurrentUser currentUser)
        {
            _freeSql = freeSql;
            _currentUser = currentUser;
        }


        public async Task<bool> CheckPermissionAsync(string permission)
        {
            long[] groups = _currentUser.Groups;

            LinPermission linPermission = await _freeSql.Select<LinPermission>().Where(r => r.Name == permission).FirstAsync();

            bool existPermission = await _freeSql.Select<LinGroupPermission>()
                .AnyAsync(r => groups.Contains(r.GroupId) && r.PermissionId == linPermission.Id);

            return existPermission;
        }
        public async Task DeletePermissionsAsync(RemovePermissionDto permissionDto)
        {
            await _freeSql.Delete<LinGroupPermission>()
                .Where(r => permissionDto.PermissionIds.Contains(r.PermissionId) && r.GroupId == permissionDto.GroupId)
                .ExecuteAffrowsAsync();
        }

        public async Task DispatchPermissions(DispatchPermissionsDto permissionDto, List<PermissionDefinition> permissionDefinitions)
        {
            List<LinGroupPermission> linPermissions = new List<LinGroupPermission>();
            permissionDto.PermissionIds.ForEach(permissionId =>
            {
                linPermissions.Add(new LinGroupPermission(permissionDto.GroupId, permissionId));
            });
            await _freeSql.Insert(linPermissions).ExecuteAffrowsAsync();
        }

        public async Task<List<LinPermission>> GetPermissionByGroupIds(List<long> groupIds)
        {
            List<long> permissionIds = _freeSql.Select<LinGroupPermission>()
                .Where(a => groupIds.Contains(a.GroupId))
                .ToList(r => r.PermissionId);

            List<LinPermission> listPermissions = await _freeSql
                .Select<LinPermission>()
                .Where(a => permissionIds.Contains(a.Id))
                .ToListAsync();

            return listPermissions;

        }

        public List<IDictionary<string, object>> StructuringPermissions(List<LinPermission> permissions)
        {
            var groupPermissions = permissions.GroupBy(r => r.Module).Select(r => new
            {
                r.Key,
                Children = r.Select(u => u.Name).ToList()
            }).ToList();

            List<IDictionary<string, object>> list = new List<IDictionary<string, object>>();

            foreach (var groupPermission in groupPermissions)
            {
                IDictionary<string, object> moduleExpandoObject = new ExpandoObject();
                List<IDictionary<string, object>> perExpandList = new List<IDictionary<string, object>>();
                groupPermission.Children.ForEach(permission =>
                {
                    IDictionary<string, object> perExpandObject = new ExpandoObject();
                    perExpandObject["module"] = groupPermission.Key;
                    perExpandObject["permission"] = permission;
                    perExpandList.Add(perExpandObject);
                });

                moduleExpandoObject[groupPermission.Key] = perExpandList;
                list.Add(moduleExpandoObject);
            }

            return list;
        }
    }
}
