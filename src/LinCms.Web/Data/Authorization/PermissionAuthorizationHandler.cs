using System.Security.Claims;
using System.Threading.Tasks;
using LinCms.Application.Contracts.Cms.Permissions;
using LinCms.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace LinCms.Web.Data.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement>
    {
        private readonly IPermissionService _permissionService;

        public PermissionAuthorizationHandler(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement)
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
                        if (await _permissionService.CheckPermissionAsync(requirement.Name))
                        {
                            context.Succeed(requirement);
                        }
                    }
                }
            }
        }
    }
}
