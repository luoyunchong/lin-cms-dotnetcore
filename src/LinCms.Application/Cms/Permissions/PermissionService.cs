using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinCms.Application.Contracts.Cms.Permissions;
using LinCms.Core.Data;
using LinCms.Core.Entities;
using LinCms.Core.Exceptions;
using Microsoft.Extensions.Caching.Memory;

namespace LinCms.Application.Cms.Permissions
{
    public class PermissionService : IPermissionService
    {
        private readonly IFreeSql _freeSql;
        private readonly IMemoryCache _cache;
        public PermissionService(IFreeSql freeSql, IMemoryCache cache)
        {
            _freeSql = freeSql;
            this._cache = cache;
        }

        public async Task RemovePermissions(PermissionDto permissionDto)
        {
            foreach (long permissionId in permissionDto.Permission)
            {
                await _freeSql.Delete<LinGroupPermission>()
                    .Where("group_id = ?GroupId and permission=?Permission",
                            new LinGroupPermission
                            {
                                PermissionId = permissionId,
                                GroupId = permissionDto.GroupId
                            }
                        )
                    .ExecuteAffrowsAsync();
            }
        }

        public async Task DispatchPermissions(PermissionDto permissionDto, List<PermissionDefinition> permissionDefinitions)
        {
            List<LinPermission> linPermissions = new List<LinPermission>();
            //foreach (long permission in permissionDto.Permission)
            //{
            //    PermissionDefinition permissionDefinition = permissionDefinitions.FirstOrDefault(r => r.== permission);
            //    if (permissionDefinition == null)
            //    {
            //        throw new LinCmsException($"异常权限:{permission}");
            //    }
            //    linPermissions.Add(new LinPermission(permission, permissionDefinition.Module, permissionDto.GroupId));
            //}

            await _freeSql.Insert(linPermissions).ExecuteAffrowsAsync();
        }
    }
}
