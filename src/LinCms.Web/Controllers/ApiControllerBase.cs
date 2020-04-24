using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSql;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers
{
    public abstract class ApiControllerBase : ControllerBase
    {
        protected ApiControllerBase()
        {
        }

    }
}
