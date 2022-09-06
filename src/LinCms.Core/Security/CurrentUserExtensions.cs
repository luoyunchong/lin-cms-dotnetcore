using System.Linq;
using System.Security.Claims;
using IGeekFan.FreeKit.Extras.Security;

namespace LinCms.Security
{
    public static class CurrentUserExtensions
    {
        public static long? FindUserId(this ICurrentUser currentUser)
        {
            return currentUser.FindUserIdToLong();
        }
        public static string[] GetGroupIds(this ICurrentUser currentUser)
        {
            return currentUser.FindClaims(LinCmsClaimTypes.GroupIds).Select(c => c.Value.ToString()).ToArray();
        }
        public static bool GetIsActive(this ICurrentUser currentUser)
        {
            Claim? claim = currentUser.FindClaim(LinCmsClaimTypes.IsActive);
            if (claim == null || claim.Value.IsNullOrWhiteSpace())
            {
                return false;
            }
            if (bool.TryParse(claim.Value, out bool isactive))
            {
                return isactive;
            }

            return false;
        }
    }
}