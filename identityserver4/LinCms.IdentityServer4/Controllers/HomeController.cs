using Microsoft.AspNetCore.Mvc;

namespace LinCms.IdentityServer4.Controllers
{
    [Route("[controller]/[action]")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public ActionResult Index()
        {
            return Redirect("/swagger");
        }
    }
}
