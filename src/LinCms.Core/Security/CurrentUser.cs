using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using LinCms.Dependency;
using Microsoft.AspNetCore.Http;

namespace LinCms.Security
{
    public class CurrentUser : ICurrentUser, ITransientDependency
    {
        private static readonly Claim[] EmptyClaimsArray = Array.Empty<Claim>();
        private readonly ClaimsPrincipal _claimsPrincipal;
        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _claimsPrincipal = httpContextAccessor.HttpContext?.User ?? Thread.CurrentPrincipal as ClaimsPrincipal;
        }
        public long? Id => _claimsPrincipal?.FindUserId();
        public string UserName => _claimsPrincipal?.FindUserName();
        public string Nickname => _claimsPrincipal.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
        public string Email => _claimsPrincipal.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        public long[] Groups => FindClaims(LinCmsClaimTypes.Groups).Select(c => c.Value.ToLong()).ToArray();

        public virtual Claim FindClaim(string claimType)
        {
            return _claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == claimType);
        }

        public virtual Claim[] FindClaims(string claimType)
        {
            return _claimsPrincipal?.Claims.Where(c => c.Type == claimType).ToArray() ?? EmptyClaimsArray;
        }

        public virtual Claim[] GetAllClaims()
        {
            return _claimsPrincipal?.Claims.ToArray() ?? EmptyClaimsArray;
        }

        public bool IsInGroup(long groupId)
        {
            return FindClaims(LinCmsClaimTypes.Groups).Any(c => c.Value.ToLong() == groupId);
        }

    }

}
