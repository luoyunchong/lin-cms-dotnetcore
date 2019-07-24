using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using LinCms.Zero.Extensions;

namespace LinCms.Zero.Security
{

    public static class ClaimsIdentityExtensions
    {
        public static int? FindUserId(this ClaimsPrincipal principal)
        {

            var userIdOrNull = principal.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdOrNull == null || userIdOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }

            return int.Parse(userIdOrNull.Value);
        }

        public static int? FindGroupId(this ClaimsPrincipal principal)
        {
            var groupOrNull = principal.Claims?.FirstOrDefault(c => c.Type == LinCmsClaimTypes.GroupId);
            if (groupOrNull == null || groupOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }
            return int.Parse(groupOrNull.Value);
        }

        public static bool? IsAdmin(this ClaimsPrincipal principal)
        {
            var isAdminOrNull = principal.Claims?.FirstOrDefault(c => c.Type == LinCmsClaimTypes.IsAdmin);
            if (isAdminOrNull == null || isAdminOrNull.Value.IsNullOrWhiteSpace())
            {
                return null;
            }
            return bool.Parse(isAdminOrNull.Value);
        }
    }
}
