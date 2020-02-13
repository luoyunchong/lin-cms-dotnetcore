using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.IdentityServer4.Controllers
{
    public class HomeController : ControllerBase
    {
        public ActionResult Index()
        {
            return Redirect("/swagger");
        }
    }
}
