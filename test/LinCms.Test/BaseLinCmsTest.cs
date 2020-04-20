using System;
using System.Net.Http;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutoMapper;
using FreeSql;
using LinCms.Web;
using LinCms.Web.Configs;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LinCms.Test
{
    public abstract class BaseLinCmsTest
    {
        protected TestServer Server { get; }
        protected HttpClient Client { get; }
        protected IServiceProvider ServiceProvider { get; }

        protected BaseLinCmsTest()
        {

            var builder = CreateHostBuilder();
            var host = builder.Build();
            host.Start();

            Server = host.GetTestServer();
            Client = host.GetTestClient();

            ServiceProvider = Server.Services;

        }

        private IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                  .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                  .ConfigureWebHostDefaults(webBuilder =>
                  {
                      webBuilder.UseStartup<Startup>().UseEnvironment("Development");
                      webBuilder.UseTestServer();
                  })
                  .ConfigureLogging(logging =>
                  {
                      logging.ClearProviders();
                      logging.SetMinimumLevel(LogLevel.Trace);
                  })
                  .ConfigureServices(ConfigureServices);
        }

        protected virtual void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {

        }

        public T GetService<T>()
        {
            return this.ServiceProvider.GetService<T>();
        }

        public  T GetRequiredService<T>()
        {
            return this.ServiceProvider.GetRequiredService<T>();
        }

    }
}
