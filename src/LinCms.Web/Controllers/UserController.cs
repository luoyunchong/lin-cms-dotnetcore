using LinCms.Web.Domain;
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

        public UserController(IFreeSql freeSql)
        {
            _freeSql = freeSql;
        }

        /// <summary>
        /// 得到当前登录人信息
        /// </summary>
        [HttpGet("information")]
        public IActionResult GetInformation()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
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
