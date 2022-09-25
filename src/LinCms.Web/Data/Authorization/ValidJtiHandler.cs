using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Data.Enums;
using LinCms.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LinCms.Data.Authorization;

/// <summary>
/// 退出登录Token 将过期
/// </summary>
public class ValidJtiHandler : AuthorizationHandler<ValidJtiRequirement>
{
    private readonly IAuditBaseRepository<BlackRecord> _blackRecordRepository;
    private readonly IHttpContextAccessor _contextAccessor;

    public ValidJtiHandler(IAuditBaseRepository<BlackRecord> blackRecordRepository, IHttpContextAccessor contextAccessor)
    {
        _blackRecordRepository = blackRecordRepository;
        _contextAccessor = contextAccessor;
    }
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ValidJtiRequirement requirement)
    {
        //检查是否登录
        AuthorizationFilterContext? filterContext = context.Resource as AuthorizationFilterContext;
        DefaultHttpContext? defaultHttpContext = context.Resource as DefaultHttpContext;
        if (context.User.Identity == null || !context.User.Identity.IsAuthenticated)
        {
            HandlerAuthenticationFailed(filterContext, "认证失败，请检查请求头或者重新登陆", ErrorCode.AuthenticationFailed);
            context.Fail();
            return;
        }

        // 检查 jti 是否在黑名单
        string? jti = _contextAccessor.HttpContext != null ? await _contextAccessor.HttpContext.GetTokenAsync("Bearer", "access_token") : null;
        var tokenExists = _blackRecordRepository.Where(r => r.Jti == jti).Any();
        if (tokenExists)
        {
            HandlerAuthenticationFailed(filterContext, "The token is expired!", ErrorCode.AuthenticationFailed);
            context.Fail();
            return;
        }
        context.Succeed(requirement);
    }

    public void HandlerAuthenticationFailed(AuthorizationFilterContext filterContext, string errorMsg, ErrorCode errorCode)
    {
        if (filterContext == null) return;
        filterContext.HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
        filterContext.Result = new JsonResult(new UnifyResponseDto(errorCode, errorMsg, filterContext.HttpContext));
    }
}