using IdentityModel.Client;
using LinCms.Web.Models.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Threading.Tasks;
using LinCms.Zero.Data.Enums;
using LinCms.Zero.Exceptions;

namespace LinCms.Web.Controllers
{
    [ApiController]
    [Route("cms/user")]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
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
            string authority = $"{_configuration["Identity:Protocol"]}://{_configuration["Identity:IP"]}:{_configuration["Identity:Port"]}";

            var client = new DiscoveryClient(authority) { Policy = { RequireHttps = false } };
            var disco = await client.GetAsync();
            var tokenClient = new TokenClient(disco.TokenEndpoint, _configuration["Service:ClientId"], _configuration["Service:ClientSecrets"]);
            TokenResponse tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync(loginInputDto.Nickname, loginInputDto.Password);
            if (tokenResponse.IsError)
            {
                throw new LinCmsException(tokenResponse.ErrorDescription);
            }

            return tokenResponse.Json;
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

            string authority = $"{_configuration["Identity:Protocol"]}://{_configuration["Identity:IP"]}:{_configuration["Identity:Port"]}";

            var client = new DiscoveryClient(authority) { Policy = { RequireHttps = false } };
            var disco = await client.GetAsync();
            var tokenClient = new TokenClient(disco.TokenEndpoint, _configuration["Service:ClientId"], _configuration["Service:ClientSecrets"]);
            var tokenResponse = await tokenClient.RequestRefreshTokenAsync(refreshToken);
            if (tokenResponse.IsError)
            {
                throw new LinCmsException(tokenResponse.ErrorDescription,ErrorCode.NotFound);
            }

            return tokenResponse.Json;
        }
    }
}