using System.Diagnostics;
using LinCms.Core.Aop.Log;
using LinCms.Core.Common;
using LinCms.Core.Data;
using LinCms.Core.Entities;
using LinCms.Core.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;

namespace LinCms.Core.Aop.Filter
{
    /// <summary>
    /// 全局日志记录
    /// </summary>
    public class LogActionFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 操作类型CRUD
        /// </summary>
        //public LogEnum LogType { get; set; }
        private readonly ICurrentUser _currentUser;

        private readonly IDiagnosticContext _diagnosticContext;

        public LogActionFilterAttribute(ICurrentUser currentUser, IDiagnosticContext diagnosticContext)
        {
            _currentUser = currentUser;
            _diagnosticContext = diagnosticContext;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            _diagnosticContext.Set("ActionArguments", JsonConvert.SerializeObject(context.ActionArguments));
            _diagnosticContext.Set("RouteData", context.ActionDescriptor.RouteValues);
            _diagnosticContext.Set("ActionName", context.ActionDescriptor.DisplayName);
            _diagnosticContext.Set("ActionId", context.ActionDescriptor.Id);
            _diagnosticContext.Set("ValidationState", context.ModelState.IsValid);

            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            //当方法或控制器上存在DisableAuditingAttribute特性标签时，不记录日志 
            // if (context.ActionDescriptor is ControllerActionDescriptor d && d.MethodInfo.IsDefined(typeof(DisableAuditingAttribute), true) ||
            //     context.Controller.GetType().IsDefined(typeof(DisableAuditingAttribute), true)
            //     )
            // {
            //     base.OnActionExecuted(context);
            //     return;
            // }

            // OtherMessage = $"参数：{ActionArguments}\n耗时：{Stopwatch.Elapsed.TotalMilliseconds} 毫秒"

            // ControllerActionDescriptor auditActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

            // AuditingLogAttribute auditingLogAttribute = auditActionDescriptor.GetCustomAttribute<AuditingLogAttribute>();
            // if (auditingLogAttribute != null)
            // {
            //      linLog.Message = auditingLogAttribute.Template;
            // }
            //
            // LinCmsAuthorizeAttribute linCmsAttribute = auditActionDescriptor.GetCustomAttribute<LinCmsAuthorizeAttribute>();
            // if (linCmsAttribute != null)
            // {
            //     linLog.Authority = linCmsAttribute.Permission;
            // }


            base.OnActionExecuted(context);
        }
    }
}