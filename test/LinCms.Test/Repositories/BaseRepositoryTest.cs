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

        protected IServiceProvider serviceProvider;
        protected readonly IWebHostEnvironment _hostingEnv;
        protected readonly IMapper _mapper;
        protected readonly IFreeSql _freeSql;
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

            _hostingEnv = serviceProvider.GetService<IWebHostEnvironment>();

            _mapper = serviceProvider.GetService<IMapper>();
            _freeSql = serviceProvider.GetService<IFreeSql>();

            serviceProvider = server.Host.Services;

        }
    }
}
