using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LinCms.Web.Data;
using LinCms.Web.Data.Aop;
using LinCms.Web.Models.Auths;
using LinCms.Web.Models.Groups;
using LinCms.Zero.Data;
using LinCms.Zero.Data.Enums;
using LinCms.Zero.Domain;
using LinCms.Zero.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers
{
    [Route("cms/admin/group")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IFreeSql _freeSql;
        private readonly IMapper _mapper;
        public GroupController(IFreeSql freeSql, IMapper mapper)
        {
            _freeSql = freeSql;
            _mapper = mapper;
        }

        [HttpGet("all")]
        public IEnumerable<LinGroup> Get()
        {
            return _freeSql.Select<LinGroup>().OrderByDescending(r => r.Id).ToList();
        }

        [HttpGet("{id}")]
        public GroupDto Get(int id)
        {
            LinGroup group = _freeSql.Select<LinGroup>().Where(r => r.Id == id).First();

            GroupDto groupDto = _mapper.Map<GroupDto>(group);

            var listAuths = _freeSql.Select<LinAuth>().Where(r => r.GroupId == id).ToList();

            groupDto.Auths = ReflexHelper.AuthsConvertToTree(listAuths);
            return groupDto;
        }

        [AuditingLog("管理员新建了一个权限组")]
        [HttpPost]
        public ResultDto Post([FromBody] CreateGroupDto inputDto)
        {
            bool exist = _freeSql.Select<LinGroup>().Any(r => r.Name == inputDto.Name);
            if (exist)
            {
                throw new LinCmsException("分组已存在，不可创建同名分组", ErrorCode.RepeatField);
            }

            LinGroup linGroup = _mapper.Map<LinGroup>(inputDto);
            List<PermissionDto> permissionDtos = ReflexHelper.GeAssemblyLinCmsAttributes();

            _freeSql.Transaction(() =>
            {
                long groupId = _freeSql.Insert(linGroup).ExecuteIdentity();

                //批量插入
                List<LinAuth> linAuths = new List<LinAuth>();
                inputDto.Auths.ForEach(r =>
                {
                    PermissionDto pdDto = permissionDtos.FirstOrDefault(u => u.Permission == r);
                    if (pdDto == null)
                    {
                        throw new LinCmsException($"不存在此权限:{r}", ErrorCode.NotFound);
                    }
                    linAuths.Add(new LinAuth(r, pdDto.Module, (int)groupId));
                });

                _freeSql.Insert<LinAuth>().AppendData(linAuths).ExecuteAffrows();
            });
            return ResultDto.Success("新建分组成功");
        }

        [HttpPut("{id}")]
        public ResultDto Put(int id, [FromBody] UpdateGroupDto updateGroupDto)
        {
            _freeSql.Update<LinGroup>(id).Set(a => new LinGroup()
            {
                Info = updateGroupDto.Info,
                Name = updateGroupDto.Name
            }).ExecuteAffrows();

            return ResultDto.Success("更新分组成功");
        }

        /// <summary>
        /// 删除一个权限组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [AuditingLog("管理员删除一个权限组")]
        public ResultDto Delete(int id)
        {
            if (!_freeSql.Select<LinGroup>(new { id = id }).Any())
            {
                return ResultDto.Error("分组不存在，删除失败");
            }

            bool exist = _freeSql.Select<LinUser>().Any(r => r.GroupId == id);
            if (exist)
            {
                throw new LinCmsException("分组下存在用户，不可删除", ErrorCode.Inoperable);
            }
            _freeSql.Transaction(() =>
            {
                //删除group拥有的权限
                _freeSql.Delete<LinAuth>().Where("group_id=?GroupId", new LinAuth() { GroupId = id }).ExecuteAffrows();
                //删除group表
                _freeSql.Delete<LinGroup>(new { id = id }).ExecuteAffrows();
            });

            return ResultDto.Success("删除分组成功");
        }

    }
}
