using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using LinCms.Application.Cms.Users;
using LinCms.Application.Contracts.Cms.Users;
using LinCms.Core.Entities;
using LinCms.Core.Extensions;
using LinCms.Core.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace LinCms.Web.Controllers.Cms
{
    [Route("cms/oauth2")]
    [ApiController]
    public class Oauth2Controller : ControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private const string LoginProviderKey = "LoginProvider";
        private readonly IConfiguration _configuration;
        private readonly IUserIdentityService _userCommunityService;
        private readonly ILogger<Oauth2Controller> _logger;

        public IFreeSql FreeSql { get; }

        public Oauth2Controller(IHttpContextAccessor contextAccessor,
                                IConfiguration configuration,
                                IFreeSql freeSql,
                                IUserIdentityService userCommunityService,
                                ILogger<Oauth2Controller> logger)
        {
            _contextAccessor = contextAccessor;
            _configuration = configuration;
            FreeSql = freeSql;
            _userCommunityService = userCommunityService;
            _logger = logger;
        }


        /// <summary>
        /// 授权成功后自动回调的地址
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="redirectUrl">授权成功后的跳转地址</param>
        /// <returns></returns>
        [HttpGet("signin-callback")]
        public async Task<IActionResult> Home(string provider, string redirectUrl = "")
        {
            if (string.IsNullOrWhiteSpace(provider))
            {
                return BadRequest();
            }

            if (!await HttpContext.IsProviderSupportedAsync(provider))
            {
                return BadRequest();
            }

            var authenticateResult = await _contextAccessor.HttpContext.AuthenticateAsync(provider);
            if (!authenticateResult.Succeeded) return Redirect(redirectUrl);
            var openIdClaim = authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier);
            if (openIdClaim == null || string.IsNullOrWhiteSpace(openIdClaim.Value))
                return Redirect(redirectUrl);
            long id = 0;
            switch (provider)
            {
                case LinUserIdentity.GitHub:
                    id = await _userCommunityService.SaveGitHubAsync(authenticateResult.Principal, openIdClaim.Value);
                    break;

                case LinUserIdentity.QQ:
                    id = await _userCommunityService.SaveQQAsync(authenticateResult.Principal, openIdClaim.Value);
                    break;
                case LinUserIdentity.WeiXin:

                    break;
                default:
                    _logger.LogError($"未知的privoder:{provider},redirectUrl:{redirectUrl}");
                    break;
            }
            List<Claim> authClaims = authenticateResult.Principal.Claims.ToList();

            LinUser user = FreeSql.Select<LinUser>().IncludeMany(r => r.LinGroups)
                .WhereCascade(r => r.IsDeleted == false).Where(r => r.Id == id).First();

            List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                    new Claim(ClaimTypes.Email,user.Email??""),
                    new Claim(ClaimTypes.GivenName,user.Nickname??""),
                    new Claim(ClaimTypes.Name,user.Username??""),
                    new Claim(LinCmsClaimTypes.IsAdmin,user.IsAdmin().ToString()),
                    new Claim(ClaimTypes.Role,user.IsAdmin()?LinGroup.Admin:"")
                };
            user.LinGroups?.ForEach(r =>
            {
                claims.Add(new Claim(LinCmsClaimTypes.Groups, r.Id.ToString()));
            });

            claims.AddRange(authClaims);
            string token = this.CreateToken(claims);
            return Redirect($"{redirectUrl}?token={token}#login-result");
        }


        /// <summary>
        /// https://localhost:5001/cms/oauth2/signin?provider=GitHub&redirectUrl=http://localhost:8080/#login-result
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="redirectUrl"></param>
        /// <returns></returns>
        [HttpGet("signin")]
        public async Task<IActionResult> SignIn(string provider, string redirectUrl)
        {
            // Note: the "provider" parameter corresponds to the external
            // authentication provider choosen by the user agent.
            if (string.IsNullOrWhiteSpace(provider))
            {
                return BadRequest();
            }

            if (!await HttpContext.IsProviderSupportedAsync(provider))
            {
                return BadRequest();
            }

            HttpRequest request = _contextAccessor.HttpContext.Request;
            string url =$"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}-callback?provider={provider}" +
                    $"&redirectUrl={redirectUrl}";

            _logger.LogInformation($"url:{url}");
            var properties = new AuthenticationProperties { RedirectUri = url };
            properties.Items[LoginProviderKey] = provider;
            return Challenge(properties, provider);

        }

        [HttpGet("signout"), HttpPost("signout")]
        public IActionResult SignOut()
        {
            // Instruct the cookies middleware to delete the local cookie created
            // when the user agent is redirected from the external identity provider
            // after a successful authentication flow (e.g Google or Facebook).
            return SignOut(new AuthenticationProperties { RedirectUri = "/" }, CookieAuthenticationDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// axios得到null，浏览器直接打开能得到github的id 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        [HttpGet("OpenId")]
        public async Task<string> OpenId(string provider)
        {
            AuthenticateResult authenticateResult = await _contextAccessor.HttpContext.AuthenticateAsync(provider);
            if (!authenticateResult.Succeeded) return null;
            Claim openIdClaim = authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier);
            return openIdClaim?.Value;

        }

        [HttpGet("GetOpenIdByToken")]
        public string GetOpenIdByToken()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        private string CreateToken(IEnumerable<Claim> Claims)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Authentication:JwtBearer:SecurityKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Authentication:JwtBearer:Issuer"],
                _configuration["Authentication:JwtBearer:Audience"],
                Claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials
            );

            return handler.WriteToken(token);
        }
    }

}