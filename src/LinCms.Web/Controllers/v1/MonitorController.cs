using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LinCms.Core.Aop.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace LinCms.Web.Controllers
{
    [Route("v1/monitor")]
    [ApiController]
    [AllowAnonymous]
    public class MonitorController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        public MonitorController(IWebHostEnvironment env)
        {
            _env = env;
        }

        /// <summary>
        /// 服务器配置信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("server-info")]
        [LinCmsAuthorize("服务器配置信息", "监控管理")]
        public ServerViewModel GetServerInfo()
        {
            return new ServerViewModel()
            {
                EnvironmentName = _env.EnvironmentName,
                OSArchitecture = RuntimeInformation.OSArchitecture.ToString(),
                ContentRootPath = _env.ContentRootPath,
                WebRootPath = _env.WebRootPath,
                FrameworkDescription = RuntimeInformation.FrameworkDescription,
                MemoryFootprint = (Process.GetCurrentProcess().WorkingSet64 / 1048576).ToString("N2") + " MB",
                WorkingTime = TimeSubTract(DateTime.Now, Process.GetCurrentProcess().StartTime)
            };
        }

        private string TimeSubTract(DateTime time1, DateTime time2)
        {
            TimeSpan subTract = time1.Subtract(time2);
            return $"{subTract.Days} 天 {subTract.Hours} 时 {subTract.Minutes} 分 ";
        }
    }

    /// <summary>
    /// 服务器VM
    /// </summary>
    public class ServerViewModel
    {
        /// <summary>
        /// 环境变量
        /// </summary>
        public string EnvironmentName { get; set; }
        /// <summary>
        /// 系统架构
        /// </summary>
        public string OSArchitecture { get; set; }
        /// <summary>
        /// ContentRootPath
        /// </summary>
        public string ContentRootPath { get; set; }
        /// <summary>
        /// WebRootPath
        /// </summary>
        public string WebRootPath { get; set; }
        /// <summary>
        /// .NET Core版本
        /// </summary>
        public string FrameworkDescription { get; set; }
        /// <summary>
        /// 内存占用
        /// </summary>
        public string MemoryFootprint { get; set; }
        /// <summary>
        /// 启动时间
        /// </summary>
        public string WorkingTime { get; set; }


    }
}
