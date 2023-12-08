using LinCms.Cms.Users;
using LinCms.Plugins.Poem.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using LinCms.Startup;
using Serilog;
using Autofac;
using LinCms.Startup.Configuration;
using IGeekFan.FreeKit.Extras.Dependency;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using LinCms.Middleware;
using Autofac.Extensions.DependencyInjection;
namespace LinCms.Test
{
    public class Startup
    {
        IConfiguration c;
        // 自定义 host 构建
        public void ConfigureHost(IHostBuilder hostBuilder)
        {
            hostBuilder
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureAppConfiguration((context, builder) =>
            {
                // 注册配置
                    builder
                          .AddJsonFile("appsettings.json")
                          ;
                })
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
                })
                .ConfigureServices((context, services) =>
                {
                    c = context.Configuration;
                    // 注册自定义服务
                })
                ;
        }

        // 支持的形式：
        // ConfigureServices(IServiceCollection services)
        // ConfigureServices(IServiceCollection services, HostBuilderContext hostBuilderContext)
        // ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services)
        public void ConfigureServices(IServiceCollection services, HostBuilderContext hostBuilderContext)
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
            // 配置日志
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

        }

        // 可以添加要用到的方法参数，会自动从注册的服务中获取服务实例，类似于 asp.net core 里 Configure 方法
        public void Configure(IServiceProvider applicationServices)
        {
            // 有一些测试数据要初始化可以放在这里
            // InitData();
        }
    }
}
