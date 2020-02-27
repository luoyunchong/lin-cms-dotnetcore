using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Application.Contracts.Cms.Groups;
using LinCms.Application.Contracts.Cms.Users;
using LinCms.Core.Data;
using LinCms.Core.Data.Enums;
using LinCms.Core.Entities;
using LinCms.Core.Exceptions;
using LinCms.Core.Extensions;
using Microsoft.AspNetCore.Http;

namespace LinCms.Application.Cms.Groups
{
    public class GroupService : IGroupService
    {
        private readonly IFreeSql _freeSql;
        private readonly IMapper _mapper;
        public GroupService(IFreeSql freeSql, IMapper mapper)
        {
            _freeSql = freeSql;
            _mapper = mapper;
        }

        public async Task<List<LinGroup>> GetListAsync()
        {
            List<LinGroup> linGroups = await _freeSql.Select<LinGroup>()
                .OrderByDescending(r => r.Id)
                .ToListAsync();

            return linGroups;
        }

        public async Task<GroupDto> GetAsync(long id)
        {
            LinGroup group = await _freeSql.Select<LinGroup>().Where(r => r.Id == id).FirstAsync();
            GroupDto groupDto = _mapper.Map<GroupDto>(group);
            return groupDto;
        }

        public async Task CreateAsync(CreateGroupDto inputDto, List<PermissionDefinition> permissionDefinitions)
        {
            bool exist = await _freeSql.Select<LinGroup>().AnyAsync(r => r.Name == inputDto.Name);
            if (exist)
            {
                throw new LinCmsException("分组已存在，不可创建同名分组", ErrorCode.RepeatField);
            }

            LinGroup linGroup = _mapper.Map<LinGroup>(inputDto);

            _freeSql.Transaction(() =>
            {
                long groupId = _freeSql.Insert(linGroup).ExecuteIdentity();

                //批量插入
                List<LinPermission> linPermissions = new List<LinPermission>();
                inputDto.Permissions.ForEach(r =>
                {
                    PermissionDefinition pdDto = permissionDefinitions.FirstOrDefault(u => u.Permission == r);
                    if (pdDto == null)
                    {
                        throw new LinCmsException($"不存在此权限:{r}", ErrorCode.NotFound);
                    }
                    //TODO
                    //linPermissions.Add(new LinGroupPermission(groupId, pe));
                });

                _freeSql.Insert<LinPermission>().AppendData(linPermissions).ExecuteAffrows();
            });
        }

        public async Task UpdateAsync(long id, UpdateGroupDto updateGroupDto)
        {
            await _freeSql.Update<LinGroup>(id).Set(a => new LinGroup()
            {
                Info = updateGroupDto.Info,
                Name = updateGroupDto.Name
            }).ExecuteAffrowsAsync();
        }

        public async Task DeleteAsync(long id)
        {
            LinGroup linGroup = await _freeSql.Select<LinGroup>(new { id = id }).FirstAsync();

            if (linGroup.IsNull())
            {
                throw new LinCmsException("分组不存在，删除失败", ErrorCode.NotFound, StatusCodes.Status404NotFound);
            }

            if (linGroup.IsStatic)
            {
                throw new LinCmsException("无法删除静态权限组!");
            }

            bool exist = await _freeSql.Select<LinUser>().AnyAsync(r => r.LinUserGroups.Any(u => u.UserId == id));
            if (exist)
            {
                throw new LinCmsException("分组下存在用户，不可删除", ErrorCode.Inoperable);
            }
            _freeSql.Transaction(() =>
            {
                //删除group拥有的权限
                _freeSql.Delete<LinGroupPermission>().Where("group_id=?GroupId",
                    new LinGroupPermission()
                    {
                        GroupId = id
                    })
                    .ExecuteAffrows();
                //删除group表
                _freeSql.Delete<LinGroup>(new { id = id }).ExecuteAffrows();
            });

        }
    }
}
