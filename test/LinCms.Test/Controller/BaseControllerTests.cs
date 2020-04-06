using System;
using System.Net.Http;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using IdentityModel.Client;
using IdentityServer4.Models;
using LinCms.Core.Exceptions;
using LinCms.Web;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using Xunit;

namespace LinCms.Test.Controller
{
    public abstract class BaseControllerTests:BaseLinCmsTest
    {
        private readonly IConfiguration _configuration;
        protected BaseControllerTests() : base()
        {
            _configuration = GetService<IConfiguration>();
           

            var builder = this.CreateHostBuilder(); ;
            var host = builder.Build();
            host.Start();

        }

        private IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<LinCms.IdentityServer4.Startup>().UseEnvironment("Development");
                    webBuilder.UseTestServer();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                });
        }
        public async Task HttpClientResourePassword()
        {
            var disco = await Client.GetDiscoveryDocumentAsync(_configuration["Service:Authority"]);
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            TokenResponse response = await Client.RequestTokenAsync(new PasswordTokenRequest
            {
                Address = disco.TokenEndpoint,
                GrantType = GrantType.ResourceOwnerPassword,
                ClientId = _configuration["Service:ClientId"],
                ClientSecret = _configuration["Service:ClientSecret"],
                Parameters =
                {
                    { "UserName","admin"},
                    { "Password","123qwe"}
                },
                Scope = _configuration["Service:Scope"]
            });
            Assert.False(response.IsError);
            Assert.NotNull(response.AccessToken);

            if (response.AccessToken == null)
            {
                throw new LinCmsException(response.Json.TryGetValue("message").ToString());
            }
            Client.SetBearerToken(response.AccessToken);
        }
    }
}
