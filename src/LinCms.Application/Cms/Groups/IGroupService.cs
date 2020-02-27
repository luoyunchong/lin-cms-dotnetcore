using System.Collections.Generic;
using LinCms.Core.Data;
using LinCms.Core.Entities;
using System.Threading.Tasks;
using LinCms.Application.Contracts.Cms.Groups;

namespace LinCms.Application.Cms.Groups
{
    public interface IGroupService
    {
        Task<List<LinGroup>> GetListAsync();
        Task<GroupDto> GetAsync(long id);
        Task CreateAsync(CreateGroupDto inputDto, List<PermissionDefinition> permissionDefinitions);
        Task UpdateAsync(long id, UpdateGroupDto inputDto);
        /// <summary>
        /// 管理员删除一个权限分组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(long id);

    }
}
