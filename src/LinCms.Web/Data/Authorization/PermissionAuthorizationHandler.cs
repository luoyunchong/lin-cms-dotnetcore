using System.Threading.Tasks;
using LinCms.Cms.Permissions;
using LinCms.Data.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LinCms.Data.Authorization;

public class PermissionAuthorizationHandler(IPermissionService permissionService) : AuthorizationHandler<ModuleAuthorizationRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ModuleAuthorizationRequirement requirement)
    {
        AuthorizationFilterContext filterContext = context.Resource as AuthorizationFilterContext;

        if (context.User.Identity == null || !context.User.Identity.IsAuthenticated)
        {
            HandlerAuthenticationFailed(filterContext, "认证失败，请检查请求头或者重新登陆", ErrorCode.AuthenticationFailed);
            context.Fail();
            return;
        }

        if (await permissionService.CheckPermissionAsync(requirement.Module, requirement.Name))
        {
            context.Succeed(requirement);
            return;
        }
        HandlerAuthenticationFailed(filterContext, $"您没有权限：{requirement.Module}-{requirement.Name}", ErrorCode.NoPermission);
    }

    public void HandlerAuthenticationFailed(AuthorizationFilterContext context, string errorMsg, ErrorCode errorCode)
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Result = new JsonResult(new UnifyResponseDto(errorCode, errorMsg, context.HttpContext));
    }
}