using System.Security.Claims;
using System.Threading.Tasks;
using LinCms.Data;

namespace LinCms.Cms.Users;

/// <summary>
/// OAuath2.0登录绑定授权
/// </summary>
public interface IOAuth2Service
{
    /// <summary>
    /// 第三方登录后，自动注册用户信息
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="openId"></param>
    /// <returns></returns>
    Task<long> SaveUserAsync(ClaimsPrincipal principal, string openId);

    /// <summary>
    ///  用户账号绑定第三方账号
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="identityType"></param>
    /// <param name="openId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<UnifyResponseDto> BindAsync(ClaimsPrincipal principal, string identityType, string openId, long userId);
}