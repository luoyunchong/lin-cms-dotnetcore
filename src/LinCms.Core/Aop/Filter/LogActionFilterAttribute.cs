using System.Linq;
using System.Text.RegularExpressions;
using LinCms.Aop.Attributes;
using LinCms.Entities;
using LinCms.IRepositories;
using LinCms.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Serilog;

namespace LinCms.Aop.Filter
{
    /// <summary>
    /// 全局日志记录
    /// </summary>
    public class LogActionFilterAttribute : ActionFilterAttribute
    {
        private readonly ICurrentUser _currentUser;
        private readonly IDiagnosticContext _diagnosticContext;
        private readonly IAuditBaseRepository<LinLog> _logRepository;
        private readonly Regex regex = new Regex("(?<=\\{)[^}]*(?=\\})");

        public LogActionFilterAttribute(ICurrentUser currentUser, IDiagnosticContext diagnosticContext, IAuditBaseRepository<LinLog> logRepository)
        {
            _currentUser = currentUser;
            _diagnosticContext = diagnosticContext;
            this._logRepository = logRepository;
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
            if (context.ActionDescriptor is ControllerActionDescriptor d && d.MethodInfo.IsDefined(typeof(DisableAuditingAttribute), true) ||
                context.Controller.GetType().IsDefined(typeof(DisableAuditingAttribute), true)
                )
            {
                base.OnActionExecuted(context);
                return;
            }
            LoggerAttribute loggerAttribute = context.ActionDescriptor.EndpointMetadata.OfType<LoggerAttribute>().FirstOrDefault();
            var linLog = new LinLog
            {
                Path = context.HttpContext.Request.Path,
                Method = context.HttpContext.Request.Method,
                StatusCode = context.HttpContext.Response.StatusCode,
                UserId = _currentUser.Id ?? 0,
                Username = _currentUser.UserName
            };

            if (loggerAttribute != null)
            {
                linLog.Message = this.ParseTemplate(loggerAttribute.Template, _currentUser, context.HttpContext.Request, context.HttpContext.Response);
            }
            else
            {
                linLog.Message = $"访问{linLog.Path}";
            }

            LinCmsAuthorizeAttribute linCmsAttribute = context.ActionDescriptor.EndpointMetadata.OfType<LinCmsAuthorizeAttribute>().FirstOrDefault();
            if (linCmsAttribute != null)
            {
                linLog.Authority = $"{linCmsAttribute.Module}  {linCmsAttribute.Permission}";
            }

            _logRepository.Insert(linLog);
            base.OnActionExecuted(context);
        }

        private string ParseTemplate(string template, ICurrentUser userDo, HttpRequest request, HttpResponse response)
        {
            foreach (Match item in regex.Matches(template))
            {
                string propertyValue = ExtractProperty(item.Value, userDo, request, response);
                template = template.Replace("{" + item.Value + "}", propertyValue);
            }
            return template;
        }

        private string ExtractProperty(string item, ICurrentUser userDo, HttpRequest request, HttpResponse response)
        {
            int i = item.LastIndexOf('.');
            string obj = item.Substring(0, i);
            string prop = item.Substring(i + 1);
            return obj switch
            {
                "user" => GetValueByPropName(userDo, prop),
                "request" => GetValueByPropName(request, prop),
                "response" => GetValueByPropName(response, prop),
                _ => "",
            };
        }

        private string GetValueByPropName<T>(T t, string prop)
        {
            return t.GetType().GetProperty(prop)?.GetValue(t, null)?.ToString();
        }
    }
}
