using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using IdentityModel.Client;
namespace LinCms.Test
{
    public class UnitTest1
    {
        [Fact]
        public async Task ResourceOwnerPasswordAsync()
        {
            var dico = DiscoveryClient.GetAsync("http://localhost:5000").Result;

            //token
            var tokenClient = new TokenClient(dico.TokenEndpoint, "pwdClient", "secret");
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("wyt", "123456");
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

        }
    }
}
