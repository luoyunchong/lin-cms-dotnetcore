using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Application.Cms.Permissions;
using LinCms.Application.Contracts.Cms.Permissions;
using LinCms.Core.Aop;
using LinCms.Core.Data;
using LinCms.Core.Entities;
using LinCms.Web.Data;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.Cms
{
    [Route("cms/admin")]
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
        public async Task<ResultDto> RemovePermissions(PermissionDto permissionDto)
        {
            await _permissionService.RemovePermissions(permissionDto);
            return ResultDto.Success("删除权限成功");
        }

        /// <summary>
        /// 分配多个权限
        /// </summary>
        /// <param name="permissionDto"></param>
        /// <returns></returns>
        [HttpPost("dispatch/patch")]
        public async Task<ResultDto> DispatchPermissions(PermissionDto permissionDto)
        {
            List<PermissionDefinition> permissionDefinitions = ReflexHelper.GeAssemblyLinCmsAttributes();
            await _permissionService.DispatchPermissions(permissionDto, permissionDefinitions);
            return ResultDto.Success("添加权限成功");
        }
    }
}