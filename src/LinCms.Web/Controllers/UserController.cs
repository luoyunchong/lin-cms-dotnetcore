using System;
using System.Threading.Tasks;
using IdentityModel.Client;
using LinCms.Web.Models.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;

namespace LinCms.Web.Controllers
{
    [ApiController]
    [Route("cms/user")]
    public class UserController : ControllerBase
    {

        public UserController()
        {
        }
        /// <summary>
        /// 注册接口
        /// </summary>
        /// <param name="value"></param>
        [HttpPost("register")]
        public void Post([FromBody] string value)
        {
        }

        /// <summary>
        /// 登录接口
        /// </summary>
        /// <param name="loginInputDto"></param>
        [HttpPost("login")]
        public async Task<JObject> Login(LoginInputDto loginInputDto)
        {
            var client = new DiscoveryClient($"http://localhost:5000") { Policy = { RequireHttps = false } };
            var disco = await client.GetAsync();
            var tokenClient = new TokenClient(disco.TokenEndpoint, "client", "secret");
            TokenResponse tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync(loginInputDto.Nickname, loginInputDto.Password);
            if (tokenResponse.IsError)
            {
                throw new Exception(tokenResponse.ErrorDescription);
            }

            return tokenResponse.Json;
        }

        /// <summary>
        /// 得到当前登录人信息
        /// </summary>
        [HttpGet("information")]
        public string GetInformation()
        {
            return "information";
        }

        /// <summary>
        /// 刷新用户的token
        /// </summary>
        /// <returns></returns>
        [HttpGet("refresh")]
        public async Task<JObject> GetRefreshToken()
        {
            string refreshToken;

            string authHeader = Request.Headers["Authorization"];

            if (authHeader != null && authHeader.StartsWith("Bearer"))
            {
                refreshToken = authHeader.Substring("Bearer ".Length).Trim();
            }
            else
            {
                //Handle what happens if that isn't the case
                throw new Exception("The authorization header is either empty or isn't Basic.");
            }

            var client = new DiscoveryClient($"http://localhost:5000") { Policy = { RequireHttps = false } };
            var disco = await client.GetAsync();
            var tokenClient = new TokenClient(disco.TokenEndpoint, "client", "secret");
            var tokenResponse = await tokenClient.RequestRefreshTokenAsync(refreshToken);
            if (tokenResponse.IsError)
            {
                throw new Exception(tokenResponse.ErrorDescription);
            }

            return tokenResponse.Json;
        }
    }
}
