using System;
using AutoMapper;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace LinCms.Test
{
    public abstract class BaseLinCmsTest
    {

        protected readonly IServiceProvider ServiceProvider;
        protected readonly IWebHostEnvironment HostingEnv;
        protected readonly IMapper Mapper;
        protected readonly IFreeSql FreeSql;
        protected BaseLinCmsTest()
        {
            var server = new TestServer(WebHost.CreateDefaultBuilder()
                .UseEnvironment("Development")
                .UseStartup<TestStartup>()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                    logging.AddConsole();
                }).UseNLog()
            ); ;
            ServiceProvider = server.Host.Services;

            HostingEnv = ServiceProvider.GetService<IWebHostEnvironment>();

            Mapper = ServiceProvider.GetService<IMapper>();
            FreeSql = ServiceProvider.GetService<IFreeSql>();

            ServiceProvider = server.Host.Services;

        }
    }
}
