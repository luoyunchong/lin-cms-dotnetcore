using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LinCms.Web.Filters
{
    /// <summary>
    /// 日志记录
    /// </summary>
    public class LogAttribute : ActionFilterAttribute
    {
        private string LogFlag { get; set; }
        /// <summary>
        /// 操作类型CRUD
        /// </summary>
        //public LogEnum LogType { get; set; }
        private string ActionArguments { get; set; }
        private Stopwatch Stopwatch { get; set; }

        public LogAttribute(string logFlag)
        {
            LogFlag = logFlag;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
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

            var user = "";
            //检测是否包含'Authorization'请求头，如果不包含则直接放行
            if (context.HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                var tokenHeader = context.HttpContext.Request.Headers["Authorization"];
                tokenHeader = tokenHeader.ToString().Substring("Bearer ".Length).Trim();

                //var tm = JwtHelper.SerializeJWT(tokenHeader);
                //user = tm.UserName;
            }


            var str = $"\n 方法：{LogFlag} \n " +
                      $"地址：{url} \n " +
                      $"方式：{method} \n " +
                      $"参数：{qs}\n " +
                      //$"结果：{res}\n " +
                      $"耗时：{Stopwatch.Elapsed.TotalMilliseconds} 毫秒";
            //Logger.Default.Process(user, LogType.GetEnumText(), str);
        }
    }
}
