using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Application.Cms.Groups;
using LinCms.Application.Contracts.Cms.Groups;
using LinCms.Application.Contracts.Cms.Groups.Dtos;
using LinCms.Core.Aop;
using LinCms.Core.Aop.Filter;
using LinCms.Core.Data;
using LinCms.Core.Entities;
using LinCms.Web.Data.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.Cms
{
    [Route("cms/admin/group")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet("all")]
        [LinCmsAuthorize("查询所有权限组","管理员")]
        public Task<List<LinGroup>> GetListAsync()
        {
            return _groupService.GetListAsync();
        }

        [HttpGet("{id}")]
        [LinCmsAuthorize("查询一个权限组及其权限","管理员")]
        public async Task<GroupDto> GetAsync(long id)
        {
            GroupDto groupDto = await _groupService.GetAsync(id);
            return groupDto;
        }

        [HttpPost]
        [LinCmsAuthorize("新建权限组","管理员")]
        public async Task<UnifyResponseDto> CreateAsync([FromBody] CreateGroupDto inputDto)
        {
            await _groupService.CreateAsync(inputDto);
            return UnifyResponseDto.Success("新建分组成功");
        }

        [HttpPut("{id}")]
        [LinCmsAuthorize("更新一个权限组","管理员")]
        public async Task<UnifyResponseDto> UpdateAsync(long id, [FromBody] UpdateGroupDto updateGroupDto)
        {
            await _groupService.UpdateAsync(id, updateGroupDto);
            return UnifyResponseDto.Success("更新分组成功");
        }

        [HttpDelete("{id}")]
        [LinCmsAuthorize("删除一个权限组","管理员")]
        public async Task<UnifyResponseDto> DeleteAsync(long id)
        {
            await _groupService.DeleteAsync(id);
            return UnifyResponseDto.Success("删除分组成功");
        }

    }
}
