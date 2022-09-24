using System.Linq;
using System.Security.Claims;
using IGeekFan.FreeKit.Extras.Security;

namespace LinCms.Security
{
    public static class CurrentUserExtensions
    {
        /// <summary>
        ///  扩展获取UserId转long
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public static long? FindUserId(this ICurrentUser currentUser)
        {
            return currentUser.FindUserIdToLong();
        }

        /// <summary>
        /// 获取分组（角色）id 数组
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public static string[] FindGroupIds(this ICurrentUser currentUser)
        {
            return currentUser.FindClaims(LinCmsClaimTypes.GroupIds).Select(c => c.Value.ToString()).ToArray();
        }

        /// <summary>
        /// 判断用户是否在此分组（角色）中
        /// </summary>
        /// <param name="currentUser"></param>
        /// <param name="groupId">分组Id</param>
        /// <returns></returns>
        public static bool IsInGroup(this ICurrentUser currentUser, long groupId)
        {
            return currentUser.FindClaims(LinCmsClaimTypes.GroupIds).Any(c => c.Value == groupId.ToString());
        }

        /// <summary>
        /// 获取用户是否激活
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public static bool FindIsActive(this ICurrentUser currentUser)
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