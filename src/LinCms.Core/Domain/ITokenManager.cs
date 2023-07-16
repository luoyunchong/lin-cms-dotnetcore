using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Security;
using LinCms.Entities;

namespace LinCms.Domain;

/// <summary>
/// Token 处理类
/// </summary>
public interface ITokenManager
{
    Task<UserAccessToken> CreateTokenAsync(LinUser user);
}