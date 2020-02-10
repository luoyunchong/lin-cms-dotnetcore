using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using IdentityModel;
using IdentityModel.Client;
using IdentityServer4.Models;
using LinCms.Application.Cms.Users;
using LinCms.Core.Aop;
using LinCms.Core.Common;
using LinCms.Core.Data;
using LinCms.Core.Data.Enums;
using LinCms.Core.Entities;
using LinCms.Core.Exceptions;
using LinCms.Application.Contracts.Cms.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace LinCms.Web.Controllers.Cms
{
    [ApiController]
    [Route("cms/user")]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;
        private readonly IUserSevice _userSevice;
        private readonly IMapper _mapper;
        public AccountController(IConfiguration configuration, ILogger<AccountController> logger, IUserSevice userSevice, IMapper mapper)
        {
            _configuration = configuration;
            _logger = logger;
            _userSevice = userSevice;
            _mapper = mapper;
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

            string authority = $"{_configuration["Identity:Protocol"]}://{_configuration["Identity:IP"]}:{_configuration["Identity:Port"]}";

            HttpClient client = new HttpClient();

            TokenResponse response = await client.RequestTokenAsync(new TokenRequest
            {
                Address = authority + "/connect/token",
                GrantType = GrantType.ResourceOwnerPassword,

                ClientId = _configuration["Service:ClientId"],
                ClientSecret = _configuration["Service:ClientSecrets"],

                Parameters =
                {
                    { "UserName",loginInputDto.Username},
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
        [AllowAnonymous]
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

            string authority = $"{_configuration["Identity:Protocol"]}://{_configuration["Identity:IP"]}:{_configuration["Identity:Port"]}";

            HttpClient client = new HttpClient();

            TokenResponse response = await client.RequestTokenAsync(new TokenRequest
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
                throw new LinCmsException("请重新登录", ErrorCode.RefreshTokenError);
            }

            return response.Json;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="registerDto"></param>
        [AuditingLog("用户注册")]
        [HttpPost("register")]
        public ResultDto Post([FromBody] RegisterDto registerDto)
        {
            LinUser user = _mapper.Map<LinUser>(registerDto);
            user.GroupId = LinConsts.Group.User;

            _userSevice.Register(user);

            return ResultDto.Success("注册成功");
        }
    }
}