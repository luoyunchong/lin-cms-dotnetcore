using System.Linq;
using System.Security.Claims;
using System.Threading;
using LinCms.Core.Common;
using LinCms.Core.Data.Options;
using LinCms.Core.Dependency;
using LinCms.Core.Entities;
using LinCms.Core.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace LinCms.Core.Security
{
    public class CurrentUser : ICurrentUser, ITransientDependency
    {
        private static readonly Claim[] EmptyClaimsArray = new Claim[0];
        private readonly ClaimsPrincipal _claimsPrincipal;
        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _claimsPrincipal = httpContextAccessor.HttpContext?.User ?? Thread.CurrentPrincipal as ClaimsPrincipal;
        }
        public long? Id => _claimsPrincipal?.FindUserId();
        public string UserName => _claimsPrincipal?.FindUserName();
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
