using System.Linq;
using System.Security.Claims;
using LinCms.Core.Extensions;

namespace LinCms.Core.Security
{

    public static class ClaimsIdentityExtensions
    {
        public static int? FindUserId(this ClaimsPrincipal principal)
        {
            Claim userIdOrNull = principal.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdOrNull == null || userIdOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }

            return int.Parse(userIdOrNull.Value);
        }

        public static bool? IsAdmin(this ClaimsPrincipal principal)
        {
            Claim isAdminOrNull = principal.Claims?.FirstOrDefault(c => c.Type == LinCmsClaimTypes.IsAdmin);
            if (isAdminOrNull == null || isAdminOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }
            return bool.Parse(isAdminOrNull.Value);
        }

        public static string FindUserName(this ClaimsPrincipal principal)
        {
            Claim userNameOrNull = principal.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            
            return userNameOrNull?.Value;
        }

    }
}
