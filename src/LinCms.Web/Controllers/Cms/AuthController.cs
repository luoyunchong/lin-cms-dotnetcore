using System.Collections.Generic;
using System.Linq;
using LinCms.Core.Aop;
using LinCms.Core.Data;
using LinCms.Core.Entities;
using LinCms.Core.Exceptions;
using LinCms.Web.Data;
using LinCms.Application.Contracts.Cms.Auths;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.Cms
{
    [Route("cms/admin")]
    [ApiController]
    [LinCmsAuthorize(Roles = LinGroup.Admin)]
    public class AuthController : ControllerBase
    {
        private readonly IFreeSql _freeSql;
        public AuthController(IFreeSql freeSql)
        {
            _freeSql = freeSql;
        }

        /// <summary>
        /// 删除某个组别的权限
        /// </summary>
        /// <param name="authDto"></param>
        /// <returns></returns>
        [HttpPost("remove")]
        public ResultDto RemoveAuths(AuthDto authDto)
        {
            foreach (string auth in authDto.Auths)
            {
                _freeSql.Delete<LinPermission>().Where("group_id = ?GroupId and auth=?Auth", new LinPermission { Name = auth, GroupId = authDto.GroupId }).ExecuteAffrows();
            }

            return ResultDto.Success("删除权限成功");
        }

        /// <summary>
        /// 分配多个权限
        /// </summary>
        /// <param name="authDto"></param>
        /// <returns></returns>
        [HttpPost("dispatch/patch")]
        public ResultDto DispatchAuths(AuthDto authDto)
        {
            List<PermissionDto> permissionDtos = ReflexHelper.GeAssemblyLinCmsAttributes();

            List<LinPermission> linAuths = new List<LinPermission>();
            foreach (string auth in authDto.Auths)
            {
                PermissionDto permission = permissionDtos.FirstOrDefault(r => r.Permission == auth);
                if (permission == null)
                {
                    throw new LinCmsException($"异常权限:{auth}");
                }
                linAuths.Add(new LinPermission(auth,permission.Module,authDto.GroupId));
            }

            _freeSql.Insert<LinPermission>(linAuths).ExecuteAffrows();

            return ResultDto.Success("添加权限成功");
        }
    }
}