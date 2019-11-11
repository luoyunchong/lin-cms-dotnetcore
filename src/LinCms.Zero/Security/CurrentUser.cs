using System.Security.Claims;
using System.Threading;
using LinCms.Zero.Common;
using LinCms.Zero.Dependency;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace LinCms.Zero.Security
{
    public class CurrentUser : ICurrentUser, ITransientDependency
    {
        private readonly ClaimsPrincipal _claimsPrincipal;
        private readonly IConfiguration _configuration;
        public CurrentUser(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _configuration = configuration;
            _claimsPrincipal = httpContextAccessor.HttpContext?.User ?? Thread.CurrentPrincipal as ClaimsPrincipal;
        }
        public long? Id => _claimsPrincipal?.FindUserId();
        public string UserName => _claimsPrincipal?.FindUserName();
        public int? GroupId => _claimsPrincipal?.FindGroupId();

        public bool? IsAdmin => _claimsPrincipal?.IsAdmin();

        public string GetFileUrl(string hash)
        {
            if (string.IsNullOrEmpty(hash)) return "";
            return _configuration[LinConsts.SITE_DOMAIN] + "/" + _configuration[LinConsts.File.STORE_DIR] + "/" + hash;
        }
    }

}
