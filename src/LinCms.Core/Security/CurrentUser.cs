using System.Linq;
using System.Security.Claims;
using System.Threading;
using LinCms.Core.Common;
using LinCms.Core.Dependency;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace LinCms.Core.Security
{
    public class CurrentUser : ICurrentUser, ITransientDependency
    {
        private static readonly Claim[] EmptyClaimsArray = new Claim[0];
        private readonly ClaimsPrincipal _claimsPrincipal;
        private readonly IConfiguration _configuration;

        public CurrentUser(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _configuration = configuration;
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

        public string GetFileUrl(string path)
        {
            if (string.IsNullOrEmpty(path)) return "";
            if (path.StartsWith("http"))
            {
                return path;
            }

            if (path.StartsWith(_configuration[LinConsts.Qiniu.PrefixPath]))
            {
                return _configuration[LinConsts.Qiniu.Host] + path;
            }

            return _configuration[LinConsts.SITE_DOMAIN] + _configuration[LinConsts.File.STORE_DIR] + "/" + path;

        }
    }

}
