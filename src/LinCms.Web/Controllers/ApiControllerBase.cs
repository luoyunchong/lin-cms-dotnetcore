using Microsoft.AspNetCore.Mvc;
using System;

namespace LinCms.Controllers
{
    public abstract class ApiControllerBase : ControllerBase
    {
        public IServiceProvider ServiceProvider { get; set; }

    }
}
