using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DotNetCore.Security;
using IdentityModel;
using IdentityModel.Client;
using IdentityServer4.Models;
using LinCms.Aop.Attributes;
using LinCms.Cms.Account;
using LinCms.Cms.Users;
using LinCms.Data;
using LinCms.Data.Enums;
using LinCms.Entities;
using LinCms.Exceptions;
using LinCms.IRepositories;
using LinCms.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace LinCms.Controllers.Cms
{
    [AllowAnonymous]
    [ApiController]
    [Route("cms/user")]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;
        private readonly IUserService _userSevice;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IJsonWebTokenService _jsonWebTokenService;
        private readonly IUserIdentityService _userIdentityService;
        public AccountController(IConfiguration configuration, ILogger<AccountController> logger, IUserService userSevice, IMapper mapper, IHttpClientFactory httpClientFactory, IJsonWebTokenService jsonWebTokenService, IUserRepository userRepository, IUserIdentityService userIdentityService)
        {
            _configuration = configuration;
            _logger = logger;
            _userSevice = userSevice;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
            _jsonWebTokenService = jsonWebTokenService;
            this._userRepository = userRepository;
            _userIdentityService = userIdentityService;
        }



        [HttpPost("jwt-login")]
        public async Task<Tokens> JwtLogin(LoginInputDto loginInputDto)
        {
            LinUser user = await _userRepository.GetUserAsync(r => r.Username == loginInputDto.Username);

            if (user == null)
            {
                throw new LinCmsException("用户不存在", ErrorCode.NotFound);
            }

            bool valid = await _userIdentityService.VerifyUserPasswordAsync(user.Id, loginInputDto.Password);

            if (!valid)
            {
                throw new LinCmsException("请输入正确密码", ErrorCode.ParameterError);
            }

            await _userRepository.UpdateLastLoginTimeAsync(user.Id);

            List<Claim> claims = new List<Claim>()
            {
                new Claim (ClaimTypes.NameIdentifier, user.Id.ToString ()),
                new Claim (ClaimTypes.Email, user.Email?? ""),
                new Claim (ClaimTypes.GivenName, user.Nickname?? ""),
                new Claim (ClaimTypes.Name, user.Username?? ""),
            };
            string token = _jsonWebTokenService.Encode(claims);
            return new Tokens(token, "");
        }

        #region Ids4登录，刷新token
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
                Policy = {
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
                Parameters = {
                        { "UserName", loginInputDto.Username },
                        { "Password", loginInputDto.Password }
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
                Policy = {
                        RequireHttps = true
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
                    { { OidcConstants.TokenRequest.RefreshToken, refreshToken }
                    }
            });

            if (response.IsError)
            {
                throw new LinCmsException("请重新登录", ErrorCode.RefreshTokenError);
            }

            return response.Json;
        }
        #endregion

        [HttpGet("logout")]
        public async Task<UnifyResponseDto> Logout()
        {
            HttpClient client = _httpClientFactory.CreateClient();
            DiscoveryDocumentResponse disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _configuration["Service:Authority"],
                Policy = {
                        RequireHttps = false
                    }
            });

            if (disco.IsError)
            {
                throw new LinCmsException(disco.Error);
            }
            RequestUrl requestUrl = new RequestUrl(disco.EndSessionEndpoint);
            string authorization = Request.Headers["Authorization"];
            string token;
            if (authorization != null && authorization.StartsWith("Bearer"))
            {
                token = authorization.Substring("Bearer ".Length).Trim();
            }
            else
            {
                return UnifyResponseDto.Success("退出登录");
            }
            
            string endsessionUrl = requestUrl.CreateEndSessionUrl(token,System.Web.HttpUtility.UrlEncode("http://localhost:8081/"));

            HttpResponseMessage response = await client.GetAsync(endsessionUrl);

            return UnifyResponseDto.Success("退出登录");
        }


        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="registerDto"></param>
        [Logger("用户注册")]
        [HttpPost("account/register")]
        public async Task<UnifyResponseDto> Register([FromBody] RegisterDto registerDto)
        {
            LinUser user = _mapper.Map<LinUser>(registerDto);
            await _userSevice.CreateAsync(user, new List<long>(), registerDto.Password);
            return UnifyResponseDto.Success("注册成功");
        }
    }
}