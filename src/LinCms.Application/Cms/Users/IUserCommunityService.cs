using System.Security.Claims;

namespace LinCms.Application.Cms.Users
{
    public interface IUserCommunityService
    {
        long SaveGitHub(ClaimsPrincipal principal, string openId);
    }
}
