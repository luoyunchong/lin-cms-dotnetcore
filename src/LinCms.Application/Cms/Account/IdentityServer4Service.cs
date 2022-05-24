using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using IdentityServer4.Models;
using LinCms.Data.Enums;
using LinCms.Exceptions;
using LinCms.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LinCms.Cms.Account
{
    public class IdentityServer4Service : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<IdentityServer4Service> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        public IdentityServer4Service(ILogger<IdentityServer4Service> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Ids4密码模式登录
        /// </summary>
        /// <param name="loginInputDto"></param>
        /// <returns></returns>
        public async Task<Tokens> LoginAsync(LoginInputDto loginInputDto)
        {
            _logger.LogInformation("IdentityServer4Login");

            HttpClient client = _httpClientFactory.CreateClient("IdentityServer4");

            DiscoveryDocumentResponse disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _configuration["Service:Authority"],
                Policy = new DiscoveryPolicy
                {
                    RequireHttps = _configuration["Service:UseHttps"].ToBoolean()
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

            JsonElement jsonElement = response.Json;

            _logger.LogInformation($"用户{loginInputDto.Username},登录成功，{JsonConvert.SerializeObject(jsonElement)}");
            return new Tokens(jsonElement.GetProperty("access_token").ToString(), jsonElement.GetProperty("refresh_token").ToString());
        }

        public async Task<Tokens> GetRefreshTokenAsync(string refreshToken)
        {
            HttpClient client = _httpClientFactory.CreateClient();
            DiscoveryDocumentResponse disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _configuration["Service:Authority"],
                Policy = new DiscoveryPolicy
                {
                    RequireHttps = _configuration["Service:UseHttps"].ToBoolean()
                }
            });

            if (disco.IsError)
            {
                throw new LinCmsException(disco.Error);
            }

            Parameters parameters = new()
            {
                new KeyValuePair<string, string>(OidcConstants.TokenRequest.RefreshToken, refreshToken)
            };

            TokenResponse response = await client.RequestTokenAsync(new TokenRequest
            {
                Address = disco.TokenEndpoint,
                GrantType = OidcConstants.GrantTypes.RefreshToken,

                ClientId = _configuration["Service:ClientId"],
                ClientSecret = _configuration["Service:ClientSecret"],
                Parameters = parameters
            });

            if (response.IsError)
            {
                _logger.LogError(response.Error + response.ErrorDescription);
                throw new LinCmsException("请重新登录", ErrorCode.RefreshTokenError);
            }

            JsonElement jsonElement = response.Json;

            return new Tokens(jsonElement.GetProperty("access_token").ToString(), jsonElement.GetProperty("refresh_token").ToString());
        }
    }
}
