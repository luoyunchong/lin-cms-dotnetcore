using System;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    public IServiceProvider? ServiceProvider { get; set; }

}