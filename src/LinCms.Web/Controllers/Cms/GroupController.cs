using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Application.Cms.Groups;
using LinCms.Application.Contracts.Cms.Groups;
using LinCms.Core.Aop;
using LinCms.Core.Data;
using LinCms.Core.Data.Enums;
using LinCms.Core.Entities;
using LinCms.Core.Exceptions;
using LinCms.Web.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.Cms
{
    [Route("cms/admin/group")]
    [ApiController]
    [LinCmsAuthorize(Roles = LinGroup.Admin)]
    public class GroupController : ControllerBase
    {
        private readonly IFreeSql _freeSql;
        private readonly IGroupService _groupService;
        public GroupController(IFreeSql freeSql, IGroupService groupService)
        {
            _freeSql = freeSql;
            _groupService = groupService;
        }

        [HttpGet("all")]
        public Task<PagedResultDto<LinGroup>> GetListAsync(PageDto input)
        {
            return _groupService.GetListAsync(input);
        }

        [HttpGet("{id}")]
        public async Task<GroupDto> GetAsync(long id)
        {
            GroupDto groupDto = await _groupService.GetAsync(id);
            List<LinPermission> listAuths = _freeSql.Select<LinPermission>().Where(r => r.GroupId == id).ToList();
            groupDto.Auths = ReflexHelper.AuthsConvertToTree(listAuths);
            return groupDto;
        }

        [AuditingLog("管理员新建了一个权限组")]
        [HttpPost]
        public async Task<ResultDto> CreateAsync([FromBody] CreateGroupDto inputDto)
        {
            await _groupService.CreateAsync(inputDto);
            return ResultDto.Success("新建分组成功");
        }

        [HttpPut("{id}")]
        public async Task<ResultDto> UpdateAsync(long id, [FromBody] UpdateGroupDto updateGroupDto)
        {
            await _groupService.UpdateAsync(id, updateGroupDto);
            return ResultDto.Success("更新分组成功");
        }

     
        [HttpDelete("{id}")]
        [AuditingLog("管理员删除一个权限分组")]
        public async Task<ResultDto> DeleteAsync(long id)
        {
            await _groupService.DeleteAsync(id);
            return ResultDto.Success("删除分组成功");
        }

    }
}
