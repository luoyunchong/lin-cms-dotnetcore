using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Application.Cms.Permissions;
using LinCms.Application.Contracts.Cms.Groups;
using LinCms.Application.Contracts.Cms.Groups.Dtos;
using LinCms.Application.Contracts.Cms.Permissions;
using LinCms.Core.Aop;
using LinCms.Core.Common;
using LinCms.Core.Data.Enums;
using LinCms.Core.Entities;
using LinCms.Core.Exceptions;
using LinCms.Core.IRepositories;
using LinCms.Core.Security;
using Microsoft.AspNetCore.Http;

namespace LinCms.Application.Cms.Groups
{
    public class GroupService : IGroupService
    {
        private readonly IFreeSql _freeSql;
        private readonly IMapper _mapper;
        private readonly IPermissionService _permissionService;
        private readonly ICurrentUser _currentUser;
        private readonly IAuditBaseRepository<LinGroup> _groupRepository;
        private readonly IAuditBaseRepository<LinUserGroup> _userGroupRepository;
        public GroupService(IFreeSql freeSql,
            IMapper mapper,
            IPermissionService permissionService,
            ICurrentUser currentUser,
            IAuditBaseRepository<LinGroup> groupRepository,
            IAuditBaseRepository<LinUserGroup> userGroupRepository
            )
        {
            _freeSql = freeSql;
            _mapper = mapper;
            _permissionService = permissionService;
            _currentUser = currentUser;
            _groupRepository = groupRepository;
            _userGroupRepository = userGroupRepository;
        }

        public async Task<List<LinGroup>> GetListAsync()
        {
            List<LinGroup> linGroups = await _groupRepository.Select
                .OrderByDescending(r => r.Id)
                .ToListAsync();

            return linGroups;
        }

        public async Task<GroupDto> GetAsync(long id)
        {
            LinGroup group = await _groupRepository.Where(r => r.Id == id).FirstAsync();
            GroupDto groupDto = _mapper.Map<GroupDto>(group);
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
                throw new LinCmsException("分组已存在，不可创建同名分组", ErrorCode.RepeatField);
            }

            LinGroup linGroup = _mapper.Map<LinGroup>(inputDto);

            using (var conn = _freeSql.Ado.MasterPool.Get())
            {
                DbTransaction transaction = conn.Value.BeginTransaction();
                try
                {
                    long groupId = _freeSql.Insert(linGroup).WithTransaction(transaction).ExecuteIdentity();

                    List<LinPermission> allPermissions = _freeSql.Select<LinPermission>().ToList();

                    List<LinGroupPermission> linPermissions = new List<LinGroupPermission>();
                    inputDto.PermissionIds.ForEach(r =>
                    {
                        LinPermission pdDto = allPermissions.FirstOrDefault(u => u.Id == r);
                        if (pdDto == null)
                        {
                            throw new LinCmsException($"不存在此权限:{r}", ErrorCode.NotFound);
                        }
                        linPermissions.Add(new LinGroupPermission(groupId, pdDto.Id));
                    });

                    _freeSql.Insert<LinGroupPermission>().WithTransaction(transaction).AppendData(linPermissions).ExecuteAffrows();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    transaction.Rollback();
                    throw;
                }
            }

            //_freeSql.Transaction(() =>
            //{
            //    long groupId = _freeSql.Insert(linGroup).ExecuteIdentity();

            //    List<LinPermission> allPermissions = _freeSql.Select<LinPermission>().ToList();

            //    List<LinGroupPermission> linPermissions = new List<LinGroupPermission>();
            //    inputDto.PermissionIds.ForEach(r =>
            //    {
            //        LinPermission pdDto = allPermissions.FirstOrDefault(u => u.Id == r);
            //        if (pdDto == null)
            //        {
            //            throw new LinCmsException($"不存在此权限:{r}", ErrorCode.NotFound);
            //        }
            //        linPermissions.Add(new LinGroupPermission(groupId, pdDto.Id));
            //    });

            //    _freeSql.Insert<LinGroupPermission>().AppendData(linPermissions).ExecuteAffrows();
            //});
        }

        public async Task UpdateAsync(long id, UpdateGroupDto updateGroupDto)
        {
            await _freeSql.Update<LinGroup>(id).Set(a => new LinGroup()
            {
                Info = updateGroupDto.Info,
                Name = updateGroupDto.Name
            }).ExecuteAffrowsAsync();
        }

        /// <summary>
        /// 删除group拥有的权限、删除group表的数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

            _freeSql.Transaction(() =>
            {
                //_freeSql.Select<LinGroupPermission>(new { GroupId = id }).ToDelete().ExecuteDeleted();
                //_freeSql.Select<LinGroup>().Where(r => r.Id == id).ToDelete().ExecuteDeleted();
                _freeSql.Delete<LinGroupPermission>(new LinGroupPermission { GroupId = id }).ExecuteDeleted();
                _freeSql.Delete<LinGroup>(new LinGroup { Id = id }).ExecuteDeleted();
            });

        }

        public async Task DeleteUserGroupAsync(long userId)
        {
            await _userGroupRepository.DeleteAsync(r => r.UserId == userId);
        }

        public bool CheckIsRootByUserId(long userId)
        {
            return _currentUser.IsInGroup(LinConsts.Group.Admin);
        }

        public List<long> GetUserGroupIdsByUserId(long userId)
        {
            return _userGroupRepository.Where(r => r.UserId == userId).ToList(r => r.GroupId);
        }

        public async Task DeleteUserGroupAsync(long userId, List<long> deleteGroupIds)
        {
            await _userGroupRepository.DeleteAsync(r => r.UserId == userId && deleteGroupIds.Contains(r.GroupId));
        }

        [UnitOfWork]
        public async Task AddUserGroupAsync(long userId, List<long> addGroupIds)
        {
            if (addGroupIds == null || addGroupIds.IsEmpty())
                return;
            bool valid = await this.CheckGroupExistByIds(addGroupIds);
            if (!valid)
            {
                throw new LinCmsException("cant't add user to non-existent group");
            }
            List<LinUserGroup> userGroups = new List<LinUserGroup>();
            addGroupIds.ForEach(groupId => { userGroups.Add(new LinUserGroup(userId, groupId)); });
            await _userGroupRepository.InsertAsync(userGroups);
        }

        private async Task<bool> CheckGroupExistById(long id)
        {
            return await _groupRepository.Where(r => r.Id == id).AnyAsync();
        }

        private async Task<bool> CheckGroupExistByIds(List<long> ids)
        {
            foreach (var id in ids)
            {
                bool valid = await CheckGroupExistById(id);
                if (!valid)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
