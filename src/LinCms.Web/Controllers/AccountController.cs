using IdentityModel.Client;
using LinCms.Web.Models.Account;
using LinCms.Web.Services.Interfaces;
using LinCms.Zero.Data.Enums;
using LinCms.Zero.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;

namespace LinCms.Web.Controllers
{
    [ApiController]
    [Route("cms/user")]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogService _logService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IConfiguration configuration, ILogService logService, ILogger<AccountController> logger)
        {
            _configuration = configuration;
            _logService = logService;
            _logger = logger;
        }

        /// <summary>
        /// 登录接口
        /// </summary>
        /// <param name="loginInputDto"></param>
        [HttpPost("login")]
        public async Task<JObject> Login(LoginInputDto loginInputDto)
        {
            _logger.LogInformation("login");

            string authority = $"{_configuration["Identity:Protocol"]}://{_configuration["Identity:IP"]}:{_configuration["Identity:Port"]}";

            var client = new HttpClient();

            var response = await client.RequestTokenAsync(new TokenRequest
            {
                Address = authority + "/connect/token",
                GrantType = GrantType.ResourceOwnerPassword,

                ClientId = _configuration["Service:ClientId"],
                ClientSecret = _configuration["Service:ClientSecrets"],

                Parameters =
                {
                    { "UserName",loginInputDto.Nickname},
                    { "Password",loginInputDto.Password}
                }
            });

            if (response.IsError)
            {
                throw new LinCmsException(response.ErrorDescription);
            }
            return response.Json;

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
                throw new Exception("The authorization header is either empty or isn't Basic.");
            }

            string authority = $"{_configuration["Identity:Protocol"]}://{_configuration["Identity:IP"]}:{_configuration["Identity:Port"]}";

            var client = new HttpClient();

            var response = await client.RequestTokenAsync(new TokenRequest
            {
                Address = authority + "/connect/token",
                GrantType = OidcConstants.GrantTypes.RefreshToken,

                ClientId = _configuration["Service:ClientId"],
                ClientSecret = _configuration["Service:ClientSecrets"],

                Parameters = new Dictionary<string, string>
                    {
                        { OidcConstants.TokenRequest.RefreshToken, refreshToken }
                    }
            });

            if (response.IsError)
            {
                throw new LinCmsException(response.ErrorDescription, ErrorCode.NotFound);
            }

            return response.Json;
        }
    }
}