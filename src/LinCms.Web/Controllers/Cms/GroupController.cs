using System.Collections.Generic;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dto;
using LinCms.Aop.Filter;
using LinCms.Cms.Groups;
using LinCms.Data;
using LinCms.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Controllers.Cms;

/// <summary>
/// 权限组
/// </summary>
[ApiExplorerSettings(GroupName = "cms")]
[Route("cms/admin/group")]
[ApiController]
public class GroupController(IGroupService groupService) : ControllerBase
{
    [HttpGet]
    [LinCmsAuthorize("查询权限组", "管理员")]
    public Task<PagedResultDto<LinGroup>> GetListAsync([FromQuery] GroupQuery query)
    {
        return groupService.GetListAsync(query);
    }

    [HttpGet("{id}")]
    [LinCmsAuthorize("查询权限组", "管理员")]
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
        return UnifyResponseDto.Success("新建权限组成功");
    }

    [HttpPut("{id}")]
    [LinCmsAuthorize("更新权限组", "管理员")]
    public async Task<UnifyResponseDto> UpdateAsync(long id, [FromBody] UpdateGroupDto updateGroupDto)
    {
        await groupService.UpdateAsync(id, updateGroupDto);
        return UnifyResponseDto.Success("更新权限组成功");
    }

    [HttpDelete("{id}")]
    [LinCmsAuthorize("删除权限组", "管理员")]
    public async Task<UnifyResponseDto> DeleteAsync(long id)
    {
        await groupService.DeleteAsync(id);
        return UnifyResponseDto.Success("删除权限组成功");
    }

}