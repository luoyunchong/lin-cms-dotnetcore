using System.Security.Claims;
using LinCms.Core.Dependency;

namespace LinCms.Core.Security
{
    public interface ICurrentUser
    {
        long? Id { get; }

        string UserName { get; }
        long[] Groups { get; }

        bool? IsAdmin { get; }

        string GetFileUrl(string hash);

        Claim FindClaim(string claimType);

        Claim[] FindClaims(string claimType);

        Claim[] GetAllClaims();


        bool IsInGroup(long groupId);
    }
}
