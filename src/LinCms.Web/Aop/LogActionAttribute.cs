using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using LinCms.Web.Services.Interfaces;
using LinCms.Zero.Data;
using LinCms.Zero.Data.Enums;
using LinCms.Zero.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LinCms.Web.Aop
{
    /// <summary>
    /// 日志记录
    /// </summary>
    public class LogActionAttribute : ActionFilterAttribute
    {
        private readonly ILogService _logService;
        /// <summary>
        /// 操作类型CRUD
        /// </summary>
        //public LogEnum LogType { get; set; }
        private string ActionArguments { get; set; }
        private Stopwatch Stopwatch { get; set; }
        private readonly ILogger<LogActionAttribute> _logger;

        public LogActionAttribute(ILogService logService, ILogger<LogActionAttribute> logger)
        {
            _logService = logService;
            _logger = logger;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // 就是这里！！
            //MiniProfiler.Current.Step($"OnActionExecuting->Begin ");
            base.OnActionExecuting(context);

            ActionArguments = Newtonsoft.Json.JsonConvert.SerializeObject(context.ActionArguments);
            Stopwatch = new Stopwatch();
            Stopwatch.Start();
        }


        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            Stopwatch.Stop();

            var url = context.HttpContext.Request.Path + context.HttpContext.Request.QueryString;
            var method = context.HttpContext.Request.Method;

            var qs = ActionArguments;

            var str = $"参数：{qs}\n耗时：{Stopwatch.Elapsed.TotalMilliseconds} 毫秒";

            string userid = context.HttpContext.User.FindFirst(JwtClaimTypes.Id)?.Value;
            string username = context.HttpContext.User.FindFirst(JwtClaimTypes.Name)?.Value;
            var linLog = new LinLog()
            {
                Authority = "",
                Method = method,
                Path = url,
                StatusCode = context.HttpContext.Response.StatusCode,
                Message = str,
                UserName = username,
                UserId = userid.IsNullOrEmpty() ? 0 : int.Parse(userid)
            };
            _logService.InsertLog(linLog);

            //记录文本日志
            _logger.LogInformation(JsonConvert.SerializeObject(linLog));
            //MiniProfiler.Current.CustomTiming($"OnActionExecuted ->", str);
            //Logger.Default.Process(user, LogType.GetEnumText(), str);
        }
    }
}
