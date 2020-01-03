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
using Microsoft.Extensions.Logging;
using NLog.Web;
using Xunit;

using Microsoft.Extensions.DependencyInjection;

namespace LinCms.Test.Repositories
{
    public abstract class BaseRepositoryTest
    {

        protected readonly IServiceProvider ServiceProvider;
        protected readonly IWebHostEnvironment HostingEnv;
        protected readonly IMapper Mapper;
        protected readonly IFreeSql FreeSql;
        protected BaseRepositoryTest()
        {
            var server = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                    logging.AddConsole();
                }).UseNLog()); ;
            ServiceProvider = server.Host.Services;

            HostingEnv = ServiceProvider.GetService<IWebHostEnvironment>();

            Mapper = ServiceProvider.GetService<IMapper>();
            FreeSql = ServiceProvider.GetService<IFreeSql>();

            ServiceProvider = server.Host.Services;

        }
    }
}
