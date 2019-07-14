using AutoMapper;
using IdentityModel;
using LinCms.Web.Models.Users;
using LinCms.Zero.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace LinCms.Web.Controllers
{
    [ApiController]
    [Route("cms/user")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IFreeSql _freeSql;
        private readonly IMapper _mapper;

        public UserController(IFreeSql freeSql, IMapper mapper)
        {
            _freeSql = freeSql;
            _mapper = mapper;
        }

        [HttpGet("get")]
        public JsonResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }

        /// <summary>
        /// 得到当前登录人信息
        /// </summary>
        [HttpGet("information")]
        public LinUserInformation GetInformation()
        {
            string id = User.FindFirst(JwtClaimTypes.Id)?.Value;

            LinUser linUser = _freeSql.Select<LinUser>().Where(r => r.Id == int.Parse(id)).First();

            return _mapper.Map<LinUserInformation>(linUser);

            //return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }

        [HttpGet("auths")]
        public void auths()
        {
            ;
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="linUser"></param>
        [HttpPost]
        public void Post([FromBody] LinUser linUser)
        {
            _freeSql.Insert(linUser).ExecuteAffrows();
        }
    }
}
