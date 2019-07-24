using System.Security.Claims;
using System.Threading;
using LinCms.Zero.Dependency;
using Microsoft.AspNetCore.Http;

namespace LinCms.Zero.Security
{
    public class CurrentUser : ICurrentUser, ITransientDependency
    {
        private readonly ClaimsPrincipal _claimsPrincipal;
        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _claimsPrincipal = httpContextAccessor.HttpContext?.User ?? Thread.CurrentPrincipal as ClaimsPrincipal;
        }
        public int? Id => _claimsPrincipal?.FindUserId();
        public int? GroupId => _claimsPrincipal?.FindGroupId();

        public bool? IsAdmin => _claimsPrincipal?.IsAdmin();
    }

}
