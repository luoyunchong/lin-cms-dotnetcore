using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

using FreeSql.Internal.ObjectPool;

using LinCms.Aop.Attributes;
using LinCms.Cms.Permissions;
using LinCms.Common;
using LinCms.Data.Enums;
using LinCms.Entities;
using LinCms.Exceptions;
using LinCms.IRepositories;

using Microsoft.AspNetCore.Http;

namespace LinCms.Cms.Groups
{
    public class GroupService : ApplicationService, IGroupService
    {
        private readonly IFreeSql _freeSql;
        private readonly IPermissionService _permissionService;
        private readonly IAuditBaseRepository<LinGroup, long> _groupRepository;
        private readonly IAuditBaseRepository<LinUserGroup, long> _userGroupRepository;
        private readonly IAuditBaseRepository<LinGroupPermission, long> _groupPermissionRepository;

        public GroupService(IFreeSql freeSql,
            IPermissionService permissionService,
            IAuditBaseRepository<LinGroup, long> groupRepository,
            IAuditBaseRepository<LinUserGroup, long> userGroupRepository,
            IAuditBaseRepository<LinGroupPermission, long> groupPermissionRepository)
        {
            _freeSql = freeSql;
            _permissionService = permissionService;
            _groupRepository = groupRepository;
            _userGroupRepository = userGroupRepository;
            _groupPermissionRepository = groupPermissionRepository;
        }

        public async Task<List<LinGroup>> GetListAsync()
        {
            List<LinGroup> linGroups = await _groupRepository.Select
                .OrderBy(r => r.SortCode)
                .ToListAsync();

            return linGroups;
        }

        public async Task<GroupDto> GetAsync(long id)
        {
            LinGroup group = await _groupRepository.Where(r => r.Id == id).FirstAsync();
            GroupDto groupDto = Mapper.Map<GroupDto>(group);
            groupDto.Permissions = await _permissionService.GetPermissionByGroupIds(new List<long>() { id });
            return groupDto;
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        public async Task CreateAsync(CreateGroupDto inputDto)
        {
            bool exist = await _groupRepository.Select.AnyAsync(r => r.Name == inputDto.Name);
            if (exist)
            {
                throw new LinCmsException($"权限组标识符{inputDto.Name}已存在，不可创建同名权限组", ErrorCode.RepeatField);
            }

            LinGroup linGroup = Mapper.Map<LinGroup>(inputDto);

            using Object<DbConnection> conn = _freeSql.Ado.MasterPool.Get();
            await using DbTransaction transaction = await conn.Value.BeginTransactionAsync();
            try
            {
                long groupId = await _freeSql.Insert(linGroup).WithTransaction(transaction).ExecuteIdentityAsync();
                List<LinPermission> allPermissions = await _freeSql.Select<LinPermission>().WithTransaction(transaction).ToListAsync();
                List<LinGroupPermission> linPermissions = new();
                inputDto.PermissionIds.ForEach(r =>
                {
                    LinPermission pdDto = allPermissions.FirstOrDefault(u => u.Id == r);
                    if (pdDto == null)
                    {
                        throw new LinCmsException($"不存在此权限:{r}", ErrorCode.NotFound);
                    }
                    linPermissions.Add(new LinGroupPermission(groupId, pdDto.Id));
                });

                await _freeSql.Insert<LinGroupPermission>()
                       .WithTransaction(transaction)
                       .AppendData(linPermissions)
                       .ExecuteAffrowsAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateAsync(long id, UpdateGroupDto updateGroupDto)
        {
            LinGroup group = await _groupRepository.Where(r => r.Id == id).FirstAsync();

            if (group.IsStatic)
            {
                if (group.Name != updateGroupDto.Name)
                {
                    throw new LinCmsException("静态权限组标识符不修改!");
                }
            }

            bool anyName = await _groupRepository.Where(r => r.Name == updateGroupDto.Name && r.Id != id).AnyAsync();
            if (anyName)
            {
                throw new LinCmsException($"权限组标识符:{updateGroupDto.Name}已存在!", ErrorCode.RepeatField);
            }

            Mapper.Map(updateGroupDto, group);
            await _groupRepository.UpdateAsync(group);
        }

        /// <summary>
        /// 删除group拥有的权限、删除group表的数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Transactional]
        public async Task DeleteAsync(long id)
        {
            LinGroup linGroup = await _groupRepository.Where(r => r.Id == id).FirstAsync();

            if (linGroup.IsNull())
            {
                throw new LinCmsException("分组不存在，删除失败", ErrorCode.NotFound, StatusCodes.Status404NotFound);
            }

            if (linGroup.IsStatic)
            {
                throw new LinCmsException("无法删除静态权限组!");
            }

            bool exist = await _userGroupRepository.Select.AnyAsync(r => r.GroupId == id);
            if (exist)
            {
                throw new LinCmsException("分组下存在用户，不可删除", ErrorCode.Inoperable);
            }

            await _groupRepository.DeleteAsync(id);
            await _groupPermissionRepository.DeleteAsync(r => r.GroupId == id);

            //_freeSql.Transaction(() =>
            //{
            //    _freeSql.Delete<LinGroupPermission>(new LinGroupPermission { GroupId = id }).ExecuteAffrows();
            //    _freeSql.Delete<LinGroup>(id).ExecuteAffrows();
            //});

        }

        [Transactional]
        public async Task DeleteUserGroupAsync(long userId)
        {
            await _userGroupRepository.DeleteAsync(r => r.UserId == userId);
        }

        public bool CheckIsRootByUserId(long userId)
        {
            return CurrentUser.IsInGroup(LinConsts.Group.Admin);
        }

        public Task<List<long>> GetGroupIdsByUserIdAsync(long userId)
        {
            return _userGroupRepository.Where(r => r.UserId == userId).ToListAsync(r => r.GroupId);
        }

        [Transactional]
        public Task DeleteUserGroupAsync(long userId, List<long> deleteGroupIds)
        {
            if (deleteGroupIds == null || deleteGroupIds.IsEmpty())
                return null;
            return _userGroupRepository.DeleteAsync(r => r.UserId == userId && deleteGroupIds.Contains(r.GroupId));
        }

        [Transactional]
        public async Task AddUserGroupAsync(long userId, List<long> addGroupIds)
        {
            if (addGroupIds == null || addGroupIds.IsEmpty())
                return;
            bool valid = await CheckGroupExistByIds(addGroupIds);
            if (!valid)
            {
                throw new LinCmsException("cant't add user to non-existent group");
            }
            List<LinUserGroup> userGroups = new();
            addGroupIds.ForEach(groupId => { userGroups.Add(new LinUserGroup(userId, groupId)); });
            await _userGroupRepository.InsertAsync(userGroups);
        }


        /// <summary>
        /// 检测新增的分组Id都存在系统中
        /// </summary>
        /// <param name="addGroupIds">新增的分组Id</param>
        /// <returns></returns>
        private async Task<bool> CheckGroupExistByIds(List<long> addGroupIds)
        {
            return (await _groupRepository.Where(r => addGroupIds.Contains(r.Id)).CountAsync()) == addGroupIds.Count;
        }
    }
}
