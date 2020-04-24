using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Application.Contracts.Cms.Groups.Dtos;
using LinCms.Core.Entities;

namespace LinCms.Application.Contracts.Cms.Groups
{
    public interface IGroupService
    {
        Task<List<LinGroup>> GetListAsync();

        Task<GroupDto> GetAsync(long id);

        Task CreateAsync(CreateGroupDto inputDto);

        Task UpdateAsync(long id, UpdateGroupDto inputDto);

        /// <summary>
        /// 管理员删除一个权限分组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DeleteAsync(long id);

        Task DeleteUserGroupAsync(long userId);
        
        /// <summary>
        /// 检查该用户是否在root分组中
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool CheckIsRootByUserId(long userId);

        /// <summary>
        /// 根据用户Id得到获得用户的所有分组id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<long> GetUserGroupIdsByUserId(long userId);

        /// <summary>
        /// 删除用户与分组直接的关联
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="deleteGroupIds">删除的分组Id</param>
        /// <returns></returns>
        Task DeleteUserGroupAsync(long userId, List<long> deleteGroupIds);

        /// <summary>
        /// 添加用户与分组直接的关联
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="addGroupIds">要新增的分组Id</param>
        /// <returns></returns>
        Task AddUserGroupAsync(long userId, List<long> addGroupIds);

    }
}
