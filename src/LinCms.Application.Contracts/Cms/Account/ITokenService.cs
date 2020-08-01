using LinCms.Security;
using System.Threading.Tasks;

namespace LinCms.Cms.Account
{
    public interface ITokenService
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="loginInputDto"></param>
        /// <returns></returns>
        Task<Tokens> LoginAsync(LoginInputDto loginInputDto);

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        Task<Tokens> GetRefreshTokenAsync(string refreshToken);
    }
}
