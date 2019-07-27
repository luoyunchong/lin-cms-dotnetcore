using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LinCms.Zero.Authorization
{
    /// <summary>
    ///  自定义固定权限编码给动态角色及用户，支持验证登录，指定角色、Policy
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class LinCmsAuthorizeAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        public string Permission { get; }
        public string Module { get; }

        public LinCmsAuthorizeAttribute()
        {

        }

        public LinCmsAuthorizeAttribute(string permission,string module)
        {
            Permission = permission;
            Module = module;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (Permission == null) return;
            var authorizationService = (IAuthorizationService)context.HttpContext.RequestServices.GetService(typeof(IAuthorizationService));
            var authorizationResult = await authorizationService.AuthorizeAsync(context.HttpContext.User, null, new OperationAuthorizationRequirement() { Name = Permission });
            if (!authorizationResult.Succeeded)
            {
                context.Result = new ForbidResult();
            }
        }

        public override string ToString()
        {
            return $"\"{base.ToString()}\",\"Permission:{Permission}\",\"Module:{Module}\",\"Roles:{Roles}\",\"Policy:{Policy}\",\"AuthenticationSchemes:{AuthenticationSchemes}\"";
        }
    }
}
