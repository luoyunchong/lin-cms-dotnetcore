using LinCms.Data;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LinCms.Cms.Users
{
    public interface IOAuth2Service
    {
        Task<long> SaveUserAsync(ClaimsPrincipal principal, string openId);

        Task<UnifyResponseDto> BindAsync(ClaimsPrincipal principal, string identityType, string openId, long userId);
    }
}
