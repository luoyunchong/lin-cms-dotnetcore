using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Application.Cms.Permissions;
using LinCms.Application.Contracts.Cms.Permissions;
using LinCms.Application.Contracts.Cms.Permissions.Dtos;
using LinCms.Core.Aop;
using LinCms.Core.Aop.Filter;
using LinCms.Core.Data;
using LinCms.Core.Entities;
using LinCms.Web.Data;
using LinCms.Web.Data.Authorization;
using LinCms.Web.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.Cms
{
    [Route("cms/admin/permission")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        public PermissionController( IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }
        
        
        /// <summary>
        /// 查询所有可分配的权限
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [LinCmsAuthorize("查询所有可分配的权限","管理员")]
        public IDictionary<string, List<PermissionDto>> GetAllPermissions()
        {
            return _permissionService.GetAllStructualPermissions();
        }

        /// <summary>
        /// 删除某个组别的权限
        /// </summary>
        /// <param name="permissionDto"></param>
        /// <returns></returns>
        [HttpPost("remove")]
        [LinCmsAuthorize("删除多个权限","管理员")]
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
        [LinCmsAuthorize("分配多个权限","管理员")]
        public async Task<UnifyResponseDto> DispatchPermissions(DispatchPermissionsDto permissionDto)
        {
            List<PermissionDefinition> permissionDefinitions = ReflexHelper.GeAssemblyLinCmsAttributes();
            await _permissionService.DispatchPermissions(permissionDto, permissionDefinitions);
            return UnifyResponseDto.Success("添加权限成功");
        }
    }
}