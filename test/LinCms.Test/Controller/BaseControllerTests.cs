using System;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using IdentityModel.Client;
using IdentityServer4.Models;
using LinCms.Web;
using LinCms.Zero.Exceptions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Web;
using Xunit;

namespace LinCms.Test.Controller
{
    public abstract class BaseControllerTests
    {
        protected HttpClient Client { get; }
        protected IServiceProvider serviceProvider;
        protected BaseControllerTests()
        {
            var server = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                    logging.AddConsole();
                }).UseNLog()); ;

            Client = server.CreateClient();


            serviceProvider = server.Host.Services; 

        }
        public async Task HttpClientResourePassword()
        {
            TokenResponse response = await Client.RequestTokenAsync(new TokenRequest
            {
                Address = "http://localhost:5000/connect/token",
                GrantType = GrantType.ResourceOwnerPassword,

                ClientId = "lin-cms-dotnetcore-client-id",
                ClientSecret = "lin-cms-dotnetcore-client-secrets",

                Parameters =
                {
                    { "UserName","admin"},
                    { "Password","123qwe"}
                }
            });
            Assert.False(response.IsError);
            Assert.NotNull(response.AccessToken);

            if (response.AccessToken == null)
            {
                throw new LinCmsException(response.Json.TryGetValue("msg").ToString());
            }
            Client.SetBearerToken(response.AccessToken);
        }
    }
}
