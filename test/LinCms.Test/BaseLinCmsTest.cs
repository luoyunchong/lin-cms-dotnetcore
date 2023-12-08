using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Autofac;
using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using IGeekFan.FreeKit.Extras.Dependency;
using LinCms.Middleware;
using LinCms.Startup.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using LinCms.Cms.Users;
using LinCms.Plugins.Poem.Services;
using LinCms.Startup;
namespace LinCms.Test
{
    public abstract class BaseLinCmsTest
    {
        protected TestServer Server { get; }
        protected HttpClient Client { get; }
        protected IServiceProvider ServiceProvider { get; }
        public static IConfiguration c { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

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

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(c)
                .Enrich.FromLogContext()
                .CreateLogger();

            return Host.CreateDefaultBuilder()
                  .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                  .ConfigureWebHostDefaults(webBuilder =>
                  {
                      //webBuilder.UseStartup<Startup.Startup>().UseEnvironment("Development");
                      webBuilder.UseTestServer()
                      .Configure(app => { 
                      
                      }).ConfigureServices(services =>
                      {
                        
                      });
                  })
                  .ConfigureLogging(logging =>
                  {
                      logging.ClearProviders();
                      logging.SetMinimumLevel(LogLevel.Trace);
                  })
                  .ConfigureServices(ConfigureServices)
                  .UseSerilog()
                  .ConfigureContainer<ContainerBuilder>((webBuilder, containerBuilder) =>
                  {
                      containerBuilder.RegisterModule(new RepositoryModule());
                      containerBuilder.RegisterModule(new ServiceModule());
                      Assembly[] currentAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(r => r.FullName != null && r.FullName.Contains("LinCms.")).ToArray();
                      containerBuilder.RegisterModule(new FreeKitModule(currentAssemblies));
                      List<Type> interceptorServiceTypes = new List<Type>()
        {
            typeof(AopCacheIntercept)
        };
                      containerBuilder.RegisterModule(new UnitOfWorkModule(currentAssemblies, interceptorServiceTypes));
                      containerBuilder.RegisterModule(new AutofacModule(c));
                  });
        }

        protected virtual void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {

            services
                .AddFreeSql(c)
                .AddLinServices(c)
                .AddCustomMvc(c)
                .AddAutoMapper(typeof(UserProfile).Assembly, typeof(PoemProfile).Assembly)
                .AddRedisClient(c)
                .AddJwtBearer(c)
                .AddSwaggerGen()//Swagger 扩展方法配置
                .AddCap(c)// 分布式事务一致性CAP
                .AddIpRateLimiting(c)//之前请注入AddRedisClient，内部实现IDistributedCache接口
                .AddGooglereCaptchav3(c)//配置Google验证码
                ;
        }

        public T GetService<T>()
        {
            return ServiceProvider.GetService<T>();
        }

        public T GetRequiredService<T>()
        {
            return ServiceProvider.GetRequiredService<T>();
        }

    }
}
