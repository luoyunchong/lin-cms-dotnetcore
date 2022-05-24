using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using CSRedis;
using DotNetCore.CAP;
using DotNetCore.CAP.Messages;
using FreeSql;
using FreeSql.Internal;
using LinCms.Data.Enums;
using LinCms.Data.Options;
using LinCms.Email;
using LinCms.Entities;
using LinCms.FreeSql;
using LinCms.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Savorboard.CAP.InMemoryMessageQueue;
using Serilog;
using ToolGood.Words;

namespace LinCms.Startup
{
    public static class DependencyInjectionExtensions
    {

        #region FreeSql 已换成AutoFac注入方式 Configuration/FreeSqlModule
        /// <summary>
        /// FreeSql
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddFreeSql(this IServiceCollection services, IConfiguration configuration)
        {
            IFreeSql fsql = new FreeSqlBuilder()
                   .UseConnectionString(configuration)
                   .UseNameConvert(NameConvertType.PascalCaseToUnderscoreWithLower)
                   .UseAutoSyncStructure(true)
                   .UseNoneCommandParameter(true)
                   .UseMonitorCommand(cmd =>
                   {
                       Trace.WriteLine(cmd.CommandText + ";");
                   }
                   )
                   .Build()
                   .SetDbContextOptions(opt => opt.EnableAddOrUpdateNavigateList = true);//联级保存功能开启（默认为关闭）

            fsql.Aop.CurdAfter += (s, e) =>
            {
                Log.Debug($"ManagedThreadId:{Thread.CurrentThread.ManagedThreadId}: FullName:{e.EntityType.FullName}" +
                          $" ElapsedMilliseconds:{e.ElapsedMilliseconds}ms, {e.Sql}");

                if (e.ElapsedMilliseconds > 200)
                {
                    //记录日志
                    //发送短信给负责人
                }
            };

            //敏感词处理
            if (configuration["AuditValue:Enable"].ToBoolean())
            {
                IllegalWordsSearch illegalWords = ToolGoodUtils.GetIllegalWordsSearch();

                fsql.Aop.AuditValue += (s, e) =>
                {
                    if (e.Column.CsType == typeof(string) && e.Value != null)
                    {
                        string oldVal = (string)e.Value;
                        string newVal = illegalWords.Replace(oldVal);
                        //第二种处理敏感词的方式
                        //string newVal = oldVal.ReplaceStopWords();
                        if (newVal != oldVal)
                        {
                            e.Value = newVal;
                        }
                    }
                };
            }

            services.AddSingleton(fsql);
            services.AddFreeRepository();
            services.AddScoped<UnitOfWorkManager>();
            fsql.GlobalFilter.Apply<IDeleteAduitEntity>("IsDeleted", a => a.IsDeleted == false);
            try
            {
                using var objPool = fsql.Ado.MasterPool.Get();
            }
            catch (Exception e)
            {
                Log.Logger.Error(e + e.StackTrace + e.Message + e.InnerException);
                return;
            }
            //在运行时直接生成表结构
            try
            {
                fsql.CodeFirst
                    .SeedData()
                    .SyncStructure(ReflexHelper.GetTypesByTableAttribute());
            }
            catch (Exception e)
            {
                Log.Logger.Error(e + e.StackTrace + e.Message + e.InnerException);
            }
        }
        #endregion

        #region 初始化 Redis配置
        public static void AddCsRedisCore(this IServiceCollection services, IConfiguration configuration)
        {

            IConfigurationSection csRediSection = configuration.GetSection("ConnectionStrings:CsRedis");
            CSRedisClient csRedisClient = new CSRedisClient(csRediSection.Value);
            //初始化 RedisHelper
            RedisHelper.Initialization(csRedisClient);
            //注册mvc分布式缓存
            services.AddSingleton<IDistributedCache>(new CSRedisCache(RedisHelper.Instance));
        }
        #endregion


        public static void AddDIServices(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddTransient<CustomExceptionMiddleWare>();

            services.AddFreeRepository();
            services.AddHttpClient("IdentityServer4");
            services.Configure<MailKitOptions>(configuration.GetSection("MailKitOptions"));
            services.Configure<FileStorageOption>(configuration.GetSection("FileStorage"));
            services.Configure<SiteOption>(configuration.GetSection("Site"));
        }

        /// <summary>
        /// 配置限流依赖的服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddIpRateLimiting(this IServiceCollection services, IConfiguration configuration)
        {
            //加载配置
            services.AddOptions();
            //从IpRateLimiting.json获取相应配置
            services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
            services.Configure<IpRateLimitPolicies>(configuration.GetSection("IpRateLimitPolicies"));
            services.AddDistributedRateLimiting();
            //配置（计数器密钥生成器）
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            return services;
        }


        #region 分布式事务一致性CAP

        public static CapOptions UseCapOptions(this CapOptions @this, IConfiguration Configuration)
        {
            IConfigurationSection defaultStorage = Configuration.GetSection("CAP:DefaultStorage");
            IConfigurationSection defaultMessageQueue = Configuration.GetSection("CAP:DefaultMessageQueue");
            if (Enum.TryParse(defaultStorage.Value, out CapStorageType capStorageType))
            {
                if (!Enum.IsDefined(typeof(CapStorageType), capStorageType))
                {
                    Log.Error($"CAP配置CAP:DefaultStorage:{defaultStorage.Value}无效");
                }

                switch (capStorageType)
                {
                    case CapStorageType.InMemoryStorage:
                        @this.UseInMemoryStorage();
                        break;
                    case CapStorageType.Mysql:
                        IConfigurationSection mySql = Configuration.GetSection($"ConnectionStrings:MySql");
                        @this.UseMySql(mySql.Value);
                        break;
                    //case CapStorageType.SqlServer:
                    //    IConfigurationSection sqlServer = Configuration.GetSection($"ConnectionStrings:SqlServer");
                    //    @this.UseSqlServer(opt =>
                    //    {
                    //        opt.ConnectionString = sqlServer.Value;
                    // //使用SQL SERVER2008才需要打开他
                    //        opt.UseSqlServer2008();
                    //    });
                    //    break;
                    default:
                        break;
                }

            }
            else
            {
                Log.Error($"CAP:DefaultStorage:{capStorageType}配置无效，仅支持InMemoryStorage，Mysql，SqlServer！更多请增加引用，修改配置项代码");
            }

            if (Enum.TryParse(defaultMessageQueue.Value, out CapMessageQueueType capMessageQueueType))
            {
                if (!Enum.IsDefined(typeof(CapMessageQueueType), capMessageQueueType))
                {
                    Log.Error($"CAP配置CAP:DefaultMessageQueue:{defaultMessageQueue.Value}无效");
                }
                IConfigurationSection configurationSection = Configuration.GetSection($"ConnectionStrings:{capMessageQueueType}");

                switch (capMessageQueueType)
                {
                    case CapMessageQueueType.InMemoryQueue:
                        @this.UseInMemoryMessageQueue();
                        break;
                    case CapMessageQueueType.RabbitMQ:
                        @this.UseRabbitMQ(options =>
                        {
                            options.HostName = Configuration["CAP:RabbitMQ:HostName"];
                            options.UserName = Configuration["CAP:RabbitMQ:UserName"];
                            options.Password = Configuration["CAP:RabbitMQ:Password"];
                            options.VirtualHost = Configuration["CAP:RabbitMQ:VirtualHost"];
                        });
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Log.Error($"CAP配置CAP:DefaultMessageQueue:{defaultMessageQueue.Value}无效");
            }

            return @this;
        }

        public static IServiceCollection AddCap(this IServiceCollection services, IConfiguration Configuration)
        {

            services.AddCap(x =>
            {
                try
                {
                    x.UseCapOptions(Configuration);
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                    throw;
                }
                x.UseDashboard();
                x.FailedRetryCount = 5;
                x.FailedThresholdCallback = (type) =>
                {
                    Log.Error($@"A message of type {type} failed after executing {x.FailedRetryCount} several times, requiring manual troubleshooting. Message name: {type.Message.GetName()}");
                };
            });

            return services;
        }

        #endregion


        /// <summary>
        /// 获取一下Scope Service 以执行 Redis的初始化
        /// </summary>
        /// <param name="serviceProvider"></param>
        public static async Task RunScopeClientPolicy(this IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                try
                {
                    var clientPolicyStore = scope.ServiceProvider.GetRequiredService<IClientPolicyStore>();
                    await clientPolicyStore.SeedAsync();

                    // get the IpPolicyStore instance
                    var ipPolicyStore = scope.ServiceProvider.GetRequiredService<IIpPolicyStore>();

                    // seed IP data from appsettings
                    await ipPolicyStore.SeedAsync();
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred.");
                }
            }
        }
    }
}
