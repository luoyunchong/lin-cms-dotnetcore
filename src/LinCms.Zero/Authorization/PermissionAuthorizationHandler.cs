using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LinCms.Zero.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace LinCms.Zero.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement>
    {

        public PermissionAuthorizationHandler()
        {
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement)
        {
            if (context.User != null)
            {
                if (context.User.IsInRole(LinGroup.Administrator))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    Claim userIdClaim = context.User.FindFirst(_ => _.Type == ClaimTypes.NameIdentifier);
                    if (userIdClaim != null)
                    {
                        if (/*_userStore.CheckPermission(int.Parse(userIdClaim.Value), requirement.Name))*/
                        true)
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
