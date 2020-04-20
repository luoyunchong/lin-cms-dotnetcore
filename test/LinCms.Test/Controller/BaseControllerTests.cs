using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Text;
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
        protected TestServer IdentityServer { get; }
        protected HttpClient IdentityClient { get; }
        protected BaseControllerTests() : base()
        {
            _configuration = GetService<IConfiguration>();
           
            var builder = this.CreateHostBuilder(); ;
            IdentityServer = new TestServer(builder);
            IdentityServer.BaseAddress = new Uri("https://localhost:5003");
            IdentityClient = IdentityServer.CreateClient();
        }

        private IWebHostBuilder CreateHostBuilder()
        {
            return  WebHost.CreateDefaultBuilder()
                            .UseEnvironment("Development")
                            // .UseUrls("https://*:5003")
                            .UseStartup<LinCms.IdentityServer4.Startup>(); 
        }

        public async Task HttpClientResourePassword()
        {
            var disco = await Client.GetDiscoveryDocumentAsync(_configuration["Service:Authority"]);
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                throw  new Exception(disco.Error);
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
        
        public  string ToParams( object source)
        {
            var buff = new StringBuilder(string.Empty);
            if (source == null)
                throw new ArgumentNullException("source", "Unable to convert object to a dictionary. The source object is null.");
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
            {
                object value = property.GetValue(source);
                if (value != null)
                {
                     buff.Append(WebUtility.UrlEncode(property.Name) + "=" + WebUtility.UrlEncode(value + "") + "&");
                }
            }
            return buff.ToString().Trim('&');
        }
    }
}
