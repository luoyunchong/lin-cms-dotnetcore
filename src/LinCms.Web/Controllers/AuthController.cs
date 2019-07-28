using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Web.Data;
using LinCms.Web.Models.Auths;
using LinCms.Zero.Authorization;
using LinCms.Zero.Data;
using LinCms.Zero.Domain;
using LinCms.Zero.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers
{
    [Route("cms/admin")]
    [ApiController]
    [LinCmsAuthorize(Roles = LinGroup.Administrator)]
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
        /// <param name="removeAuthDto"></param>
        /// <returns></returns>
        [HttpPost("remove")]
        public ResultDto RemoveAuths(AuthDto removeAuthDto)
        {
            foreach (var auth in removeAuthDto.Auths)
            {
                _freeSql.Delete<LinAuth>().Where("group_id = ?GroupId and auth=?Auth", new LinAuth { Auth = auth, GroupId = removeAuthDto.GroupId }).ExecuteAffrows();
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

            List<LinAuth> linAuths = new List<LinAuth>();
            foreach (var auth in authDto.Auths)
            {
                PermissionDto permission = permissionDtos.FirstOrDefault(r => r.Permission == auth);
                if (permission == null)
                {
                    throw new LinCmsException($"异常权限:{auth}");
                }
                linAuths.Add(new LinAuth(auth,permission.Module,authDto.GroupId));
            }

            _freeSql.Insert<LinAuth>(linAuths).ExecuteAffrows();

            return ResultDto.Success("添加权限成功");
        }
    }
}