using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using IdentityModel.Client;
using IdentityServer4.Models;
using LinCms.Zero.Domain;
using LinCms.Zero.Exceptions;

namespace LinCms.Test
{
    public class IdentityServerTest
    {
        [Fact]
        public async Task ResourceOwnerPasswordAsync()
        {
            var dico = DiscoveryClient.GetAsync("http://localhost:5000").Result;

            //token
            var tokenClient = new TokenClient(dico.TokenEndpoint, "lin-cms-dotnetcore-client-id", "lin-cms-dotnetcore-client-secrets");
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("string", "string",extra:new Dictionary<string,string>
            {
                {"__tenant","123" }
            });
            if (tokenResponse.IsError)
            {
                return;
            }


            var httpClient = new HttpClient();
            httpClient.SetBearerToken(tokenResponse.AccessToken);

            var response = httpClient.GetAsync("http://localhost:5001/api/values").Result;
            if (!response.IsSuccessStatusCode)
            {
                string r = response.Content.ReadAsStringAsync().Result;
            }
        }
        
        [Fact]
        public async Task HttpClientResourePassword()
        {

            var client = new HttpClient();

            var response = await client.RequestTokenAsync(new TokenRequest
            {
                Address = "http://localhost:5000/connect/token",
                GrantType = GrantType.ResourceOwnerPassword,

                ClientId = "lin-cms-dotnetcore-client-id",
                ClientSecret = "lin-cms-dotnetcore-client-secrets",

                Parameters =
                {
                    { "UserName","string"},
                    { "Password","string"}
                }
            });

            if (response.IsError)
            {
                throw new LinCmsException(response.ErrorDescription);
            }

            Assert.False(response.IsError);
        }

        [Fact]
        public void TestPermission()
        {
            var g=LinAuth.g();


        }

        [Fact]
        public void GetAllPermissionAttirbute()
        {

        }
    }
}
