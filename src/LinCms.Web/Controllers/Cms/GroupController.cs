using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Aop.Filter;
using LinCms.Cms.Groups;
using LinCms.Data;
using LinCms.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Controllers.Cms;

/// <summary>
/// 分组
/// </summary>
[ApiExplorerSettings(GroupName = "cms")]
[Route("cms/admin/group")]
[ApiController]
public class GroupController(IGroupService groupService) : ControllerBase
{
    [HttpGet("all")]
    [LinCmsAuthorize("查询所有权限组", "管理员")]
    public Task<List<LinGroup>> GetListAsync()
    {
        return groupService.GetListAsync();
    }

    [HttpGet("{id}")]
    [LinCmsAuthorize("查询一个权限组及其权限", "管理员")]
    public async Task<GroupDto> GetAsync(long id)
    {
        GroupDto groupDto = await groupService.GetAsync(id);
        return groupDto;
    }

    [HttpPost]
    [LinCmsAuthorize("新建权限组", "管理员")]
    public async Task<UnifyResponseDto> CreateAsync([FromBody] CreateGroupDto inputDto)
    {
        await groupService.CreateAsync(inputDto);
        return UnifyResponseDto.Success("新建分组成功");
    }

    [HttpPut("{id}")]
    [LinCmsAuthorize("更新一个权限组", "管理员")]
    public async Task<UnifyResponseDto> UpdateAsync(long id, [FromBody] UpdateGroupDto updateGroupDto)
    {
        await groupService.UpdateAsync(id, updateGroupDto);
        return UnifyResponseDto.Success("更新分组成功");
    }

    [HttpDelete("{id}")]
    [LinCmsAuthorize("删除一个权限组", "管理员")]
    public async Task<UnifyResponseDto> DeleteAsync(long id)
    {
        await groupService.DeleteAsync(id);
        return UnifyResponseDto.Success("删除分组成功");
    }

}