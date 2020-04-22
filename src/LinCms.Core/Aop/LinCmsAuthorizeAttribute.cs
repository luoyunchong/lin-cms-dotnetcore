using System;
using System.Security.Claims;
using System.Threading.Tasks;
using LinCms.Core.Data;
using LinCms.Core.Data.Enums;
using LinCms.Core.Entities;
using LinCms.Core.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LinCms.Core.Aop
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
        public LinCmsAuthorizeAttribute(string permission, string module, string role) : this(permission, module)
        {
            base.Roles = role;
        }

        public LinCmsAuthorizeAttribute(string permission, string module)
        {
            Permission = permission;
            Module = module;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            ClaimsPrincipal claimsPrincipal = context.HttpContext.User;

            if (!claimsPrincipal.Identity.IsAuthenticated)
            {
                HandlerAuthenticationFailed(context, "认证失败，请检查请求头或者重新登陆");
                return;
            }


            if (Permission == null && Roles == LinGroup.Admin)
            {
                ICurrentUser currentUser = (ICurrentUser)context.HttpContext.RequestServices.GetService(typeof(ICurrentUser));

                if (currentUser.IsAdmin != true)
                {
                    HandlerAuthenticationFailed(context, "只有超级管理员可操作");
                    return;
                }
            }

            IAuthorizationService authorizationService = (IAuthorizationService)context.HttpContext.RequestServices.GetService(typeof(IAuthorizationService));
            AuthorizationResult authorizationResult = await authorizationService.AuthorizeAsync(context.HttpContext.User, null, new OperationAuthorizationRequirement() { Name = Permission });
            if (!authorizationResult.Succeeded)
            {
                //通过报业务异常，统一返回结果，平均执行速度在500ms以上，直接返回无权限，则除第一次访问慢外，基本在80ms左右。
                //throw new LinCmsException("权限不够，请联系超级管理员获得权限", ErrorCode.AuthenticationFailed, StatusCodes.Status401Unauthorized);

                HandlerAuthenticationFailed(context, "权限不够，请联系超级管理员获得权限");
            }
        }

        public void HandlerAuthenticationFailed(AuthorizationFilterContext context, string errorMsg)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Result = new JsonResult(
                new UnifyResponseDto(ErrorCode.AuthenticationFailed, errorMsg, context.HttpContext)
            );
        }

        public override string ToString()
        {
            return $"\"{base.ToString()}\",\"Permission:{Permission}\",\"Module:{Module}\",\"Roles:{Roles}\",\"Policy:{Policy}\",\"AuthenticationSchemes:{AuthenticationSchemes}\"";
        }
    }
}
