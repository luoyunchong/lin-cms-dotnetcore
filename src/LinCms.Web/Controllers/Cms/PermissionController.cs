using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Aop.Filter;
using LinCms.Cms.Permissions;
using LinCms.Data;
using LinCms.Utils;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Controllers.Cms;

/// <summary>
/// 权限
/// </summary>
[ApiExplorerSettings(GroupName = "cms")]
[Route("cms/admin/permission")]
[ApiController]
public class PermissionController(IPermissionService permissionService) : ControllerBase
{
    /// <summary>
    /// 查询所有可分配的权限
    /// </summary>
    /// <returns></returns>
    [HttpGet("tree")]
    [LinCmsAuthorize("分配权限", "管理员")]
    public async Task<List<PermissionTreeNode>> GetPermissionTreeNodes()
    {
        return await permissionService.GetPermissionTreeNodes();
    }

    /// <summary>
    /// 删除某个组别的权限
    /// </summary>
    /// <param name="permissionDto"></param>
    /// <returns></returns>
    [HttpPost("remove")]
    [LinCmsAuthorize("分配权限", "管理员")]
    public async Task<UnifyResponseDto> RemovePermissions(RemovePermissionDto permissionDto)
    {
        await permissionService.DeletePermissionsAsync(permissionDto);
        return UnifyResponseDto.Success("删除权限成功");
    }

    /// <summary>
    /// 分配多个权限
    /// </summary>
    /// <param name="permissionDto"></param>
    /// <returns></returns>
    [HttpPost("dispatch/batch")]
    [LinCmsAuthorize("分配权限", "管理员")]
    public async Task<UnifyResponseDto> DispatchPermissions(DispatchPermissionsDto permissionDto)
    {
        List<PermissionDefinition> permissionDefinitions = ReflexHelper.GetAssemblyLinCmsAttributes();
        await permissionService.DispatchPermissions(permissionDto, permissionDefinitions);
        return UnifyResponseDto.Success("添加权限成功");
    }


    [HttpPost]
    [LinCmsAuthorize("权限管理", "管理员")]
    public async Task<UnifyResponseDto> CreateAsync([FromBody] PermissioCreateUpdateDto permissionDto)
    {
        await permissionService.CreateAsync(permissionDto);
        return UnifyResponseDto.Success("新增权限成功");
    }

    [HttpPut("{id}")]
    [LinCmsAuthorize("权限管理", "管理员")]
    public async Task<UnifyResponseDto> UpdateAsync(int id, [FromBody] PermissioCreateUpdateDto permissionDto)
    {
        await permissionService.UpdateAsync(id, permissionDto);
        return UnifyResponseDto.Success("修改权限成功");
    }

    [HttpDelete("{id}")]
    [LinCmsAuthorize("权限管理", "管理员")]
    public async Task<UnifyResponseDto> UpdateAsync(int id)
    {
        await permissionService.DeleteAsync(id);
        return UnifyResponseDto.Success("删除权限成功");
    }
}