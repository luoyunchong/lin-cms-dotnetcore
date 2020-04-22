using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using AutoMapper;

using IdentityModel;
using IdentityModel.Client;

using IdentityServer4.Models;

using LinCms.Application.Cms.Users;
using LinCms.Application.Contracts.Cms.Account;
using LinCms.Application.Contracts.Cms.Users;
using LinCms.Core.Aop;
using LinCms.Core.Data;
using LinCms.Core.Data.Enums;
using LinCms.Core.Entities;
using LinCms.Core.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Linq;

namespace LinCms.Web.Controllers.Cms
{
    [AllowAnonymous]
    [ApiController]
    [Route("cms/user")]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;
        private readonly IUserService _userSevice;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;
        public AccountController(IConfiguration configuration, ILogger<AccountController> logger, IUserService userSevice, IMapper mapper, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _logger = logger;
            _userSevice = userSevice;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// 登录接口
        /// </summary>
        /// <param name="loginInputDto">用户名/密码：admin/123qwe</param>
        [DisableAuditing]
        [HttpPost("login")]
        public async Task<JObject> Login(LoginInputDto loginInputDto)
        {
            _logger.LogInformation("login");

            HttpClient client = _httpClientFactory.CreateClient();

            DiscoveryDocumentResponse disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _configuration["Service:Authority"],
                Policy =
                {
                    RequireHttps = false
                }
             });

            if (disco.IsError)
            {
                throw new LinCmsException(disco.Error);
            }

            TokenResponse response = await client.RequestTokenAsync(new PasswordTokenRequest()
            {
                Address = disco.TokenEndpoint,
                GrantType = GrantType.ResourceOwnerPassword,
                ClientId = _configuration["Service:ClientId"],
                ClientSecret = _configuration["Service:ClientSecret"],
                Parameters =
                {
                    { "UserName",loginInputDto.Username},
                    { "Password",loginInputDto.Password}
                },
                Scope = _configuration["Service:Name"],
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
                throw new LinCmsException(" 请先登录.", ErrorCode.RefreshTokenError);
            }

            HttpClient client = _httpClientFactory.CreateClient();

            DiscoveryDocumentResponse disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _configuration["Service:Authority"],
                Policy =
                {
                    RequireHttps = false
                }
            });

            if (disco.IsError)
            {
                throw new LinCmsException(disco.Error);
            }

            TokenResponse response = await client.RequestTokenAsync(new TokenRequest
            {
                Address = disco.TokenEndpoint,
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
                throw new LinCmsException("请重新登录", ErrorCode.RefreshTokenError);
            }

            return response.Json;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="registerDto"></param>
        [AuditingLog("用户注册")]
        [HttpPost("account/register")]
        public UnifyResponseDto Post([FromBody] RegisterDto registerDto)
        {
            LinUser user = _mapper.Map<LinUser>(registerDto);

            _userSevice.Register(user, new List<long>(), registerDto.Password);

            return UnifyResponseDto.Success("注册成功");
        }
    }
}