using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.Client;
using IdentityServer4.Models;
using LinCms.Data.Enums;
using IGeekFan.FreeKit.Extras.Dependency;
using IGeekFan.FreeKit.Extras.Security;
using LinCms.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LinCms.Cms.Account;

[DisableConventionalRegistration]
public class IdentityServer4Service(ILogger<IdentityServer4Service> logger, IConfiguration configuration,
        IHttpClientFactory httpClientFactory)
    : ITokenService
{
    /// <summary>
    /// Ids4密码模式登录
    /// </summary>
    /// <param name="loginInputDto"></param>
    /// <returns></returns>
    public async Task<UserAccessToken> LoginAsync(LoginInputDto loginInputDto)
    {
        logger.LogInformation("IdentityServer4Login");

        HttpClient client = httpClientFactory.CreateClient("IdentityServer4");

        DiscoveryDocumentResponse disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = configuration["Service:Authority"],
            Policy = new DiscoveryPolicy
            {
                RequireHttps = configuration["Service:UseHttps"].ToBoolean()
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
            ClientId = configuration["Service:ClientId"],
            ClientSecret = configuration["Service:ClientSecret"],
            Parameters = {
                { "UserName", loginInputDto.Username },
                { "Password", loginInputDto.Password }
            },
            Scope = configuration["Service:Name"],
        });

        if (response.IsError)
        {
            throw new LinCmsException(response.ErrorDescription);
        }

        var jsonElement = response.Json;

        logger.LogInformation($"用户{loginInputDto.Username},登录成功，{JsonConvert.SerializeObject(jsonElement)}");
        return new UserAccessToken(jsonElement.GetProperty("access_token").ToString(), jsonElement.GetProperty("refresh_token").ToString(), 7200, "Bearer", 24 * 60 * 60 * 7);
    }

    public async Task<UserAccessToken> GetRefreshTokenAsync(string refreshToken)
    {
        HttpClient client = httpClientFactory.CreateClient();
        DiscoveryDocumentResponse disco = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
        {
            Address = configuration["Service:Authority"],
            Policy = new DiscoveryPolicy
            {
                RequireHttps = configuration["Service:UseHttps"].ToBoolean()
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

            ClientId = configuration["Service:ClientId"],
            ClientSecret = configuration["Service:ClientSecret"],
            Parameters = parameters
        });

        if (response.IsError)
        {
            logger.LogError(response.Error + response.ErrorDescription);
            throw new LinCmsException("请重新登录", ErrorCode.RefreshTokenError);
        }

        var jsonElement = response.Json;

        return new UserAccessToken(jsonElement.GetProperty("access_token").ToString(), jsonElement.GetProperty("refresh_token").ToString(), 7200, "Bearer", 24 * 60 * 60 * 7);
    }
}