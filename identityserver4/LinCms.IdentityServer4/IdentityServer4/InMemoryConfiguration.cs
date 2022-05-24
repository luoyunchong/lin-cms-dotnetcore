using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace LinCms.IdentityServer4.IdentityServer4
{
    public static class InMemoryConfiguration
    {
        public static IConfiguration Configuration { get; set; }
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId()
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                //Configuration["Service:Scope"]在AddJwtBearerAudience
                new ApiResource(Configuration["Service:Name"], Configuration["Service:DocName"])
                {
                    Scopes= { Configuration["Service:Name"] }
                }
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new ApiScope[]
             {
               new ApiScope(Configuration["Service:Name"], "scope1")
             };
        }

        public static ICollection<string> NewGrantTypes => new string[3]
        {
                "authorization_code",
                "password",
                "client_credentials"
        };

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                // resource owner password grant client
                new Client
                {
                    ClientId = Configuration["Service:ClientId"],
                    //AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedGrantTypes =NewGrantTypes,
                    AllowOfflineAccess = true,
                    //UpdateAccessTokenClaimsOnRefresh = true,
                    AccessTokenLifetime = 3600 * 24 * 15, //15天      //5  设置 5s，验证过期策略。
                    SlidingRefreshTokenLifetime = 1296000, //15天
                    ClientSecrets =
                    {
                        new Secret(Configuration["Service:ClientSecret"].Sha256())
                    },
                    AllowedScopes =
                    {
                        Configuration["Service:Name"],
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        IdentityServerConstants.StandardScopes.OpenId,
                    },
                    RedirectUris = new[] {
                        "http://localhost:5000/oauth-receiver.html",
                        "https://localhost:5001/oauth-receiver.html",
                        "http://api.igeekfan.cn/oauth-receiver.html",
                        "https://api.igeekfan.cn/oauth-receiver.html",
                        "http://localhost:5000/swagger/oauth2-redirect.html",
                        "https://localhost:5001/swagger/oauth2-redirect.html",
                        "https://api.igeekfan.cn/swagger/oauth2-redirect.html",
                    },
                    RequirePkce=false
                }
            };
        }

    };
}
