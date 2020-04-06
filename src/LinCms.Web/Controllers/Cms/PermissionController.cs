using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Application.Cms.Permissions;
using LinCms.Application.Contracts.Cms.Permissions;
using LinCms.Application.Contracts.Cms.Permissions.Dtos;
using LinCms.Core.Aop;
using LinCms.Core.Data;
using LinCms.Core.Entities;
using LinCms.Web.Data;
using LinCms.Web.Utils;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.Cms
{
    [Route("cms/admin/permission")]
    [ApiController]
    [LinCmsAuthorize(Roles = LinGroup.Admin)]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        public PermissionController( IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        /// <summary>
        /// 删除某个组别的权限
        /// </summary>
        /// <param name="permissionDto"></param>
        /// <returns></returns>
        [HttpPost("remove")]
        public async Task<UnifyResponseDto> RemovePermissions(RemovePermissionDto permissionDto)
        {
            await _permissionService.DeletePermissionsAsync(permissionDto);
            return UnifyResponseDto.Success("删除权限成功");
        }

        /// <summary>
        /// 分配多个权限
        /// </summary>
        /// <param name="permissionDto"></param>
        /// <returns></returns>
        [HttpPost("dispatch/batch")]
        public async Task<UnifyResponseDto> DispatchPermissions(DispatchPermissionsDto permissionDto)
        {
            List<PermissionDefinition> permissionDefinitions = ReflexHelper.GeAssemblyLinCmsAttributes();
            await _permissionService.DispatchPermissions(permissionDto, permissionDefinitions);
            return UnifyResponseDto.Success("添加权限成功");
        }
    }
}