using System.Security.Claims;
using System.Threading.Tasks;
using LinCms.Web.Services.Cms.Interfaces;
using LinCms.Zero.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace LinCms.Web.Data.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement>
    {
        private readonly IUserSevice _userService;

        public PermissionAuthorizationHandler()
        {
        }

        public PermissionAuthorizationHandler(IUserSevice userService)
        {
            _userService = userService;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement)
        {
            if (context.User != null)
            {
                if (context.User.IsInRole(LinGroup.Admin))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    Claim userIdClaim = context.User.FindFirst(_ => _.Type == ClaimTypes.NameIdentifier);
                    if (userIdClaim != null)
                    {
                        if (_userService.CheckPermission(int.Parse(userIdClaim.Value), requirement.Name))
                        {
                            context.Succeed(requirement);
                        }
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
