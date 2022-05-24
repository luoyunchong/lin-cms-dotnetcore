using System;
using System.Threading.Tasks;
using LinCms.Data.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LinCms.Aop.Filter
{
    /// <summary>
    ///  自定义固定权限编码给动态角色及用户，支持验证登录，退出登录，权限。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class LinCmsAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public string Permission { get; }
        public string Module { get; }

        public LinCmsAuthorizeAttribute(string permission, string module)
        {
            Permission = permission;
            Module = module;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            //ICurrentUser currentUser = (ICurrentUser)context.HttpContext.RequestServices.GetService(typeof(ICurrentUser));
            //if (currentUser.IsInGroup(LinConsts.Group.Admin))
            //{
            //    return;
            //}

            IAuthorizationService authorizationService = (IAuthorizationService)context.HttpContext.RequestServices.GetService(typeof(IAuthorizationService));
            AuthorizationResult validJti = await authorizationService.AuthorizeAsync(context.HttpContext.User, context, new ValidJtiRequirement());
            if (!validJti.Succeeded)
            {
                return;
            }

            await authorizationService.AuthorizeAsync(context.HttpContext.User, context, new ModuleAuthorizationRequirement(Module, Permission));
        }

        public override string ToString()
        {
            return $"\"{base.ToString()}\",\"Permission:{Permission}\",\"Module:{Module}\",";
        }
    }
}
