using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using AspNetCoreRateLimit;
using CSRedis;
using CSRedis.Internal.ObjectPool;
using DotNetCore.CAP;
using DotNetCore.CAP.Messages;
using DotNetCore.Security;
using FreeSql;
using FreeSql.Internal;
using IGeekFan.Localization.FreeSql.Models;
using LinCms.Application.Cms.Files;
using LinCms.Application.Contracts.Cms.Files;
using LinCms.Core.Aop.Middleware;
using LinCms.Core.Common;
using LinCms.Core.Data.Enums;
using LinCms.Core.Data.Options;
using LinCms.Core.Entities;
using LinCms.Core.Entities.Base;
using LinCms.Core.Security;
using LinCms.Infrastructure.FreeSql;
using LinCms.Web.Data.Authorization;
using LinCms.Web.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Savorboard.CAP.InMemoryMessageQueue;
using Serilog;
using ToolGood.Words;

namespace LinCms.Web.Startup
{
    public static class DependencyInjectionExtensions
    {

        #region FreeSql
        /// <summary>
        /// FreeSql
        /// </summary>
        /// <param name="services"></param>
        public static void AddContext(this IServiceCollection services, IConfiguration configuration)
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
        public static IServiceCollection AddFreeRepository(this IServiceCollection services)
        {
            services.TryAddScoped(typeof(IBaseRepository<>), typeof(GuidRepository<>));
            services.TryAddScoped(typeof(BaseRepository<>), typeof(GuidRepository<>));
            services.TryAddScoped(typeof(IBaseRepository<,>), typeof(DefaultRepository<,>));
            services.TryAddScoped(typeof(BaseRepository<,>), typeof(DefaultRepository<,>));
            return services;
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

        public static void AddSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHash(10000, 128);
            services.AddCryptography("lin-cms-dotnetcore-cryptography");
            services.AddJsonWebToken(
                new JsonWebTokenSettings(
                        configuration["Authentication:JwtBearer:SecurityKey"],
                        new TimeSpan(30, 0, 0, 0),
                        configuration["Authentication:JwtBearer:Audience"],
                        configuration["Authentication:JwtBearer:Issuer"]
                    )
                );
        }

        public static void AddDIServices(this IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            //services.AddTransient<CustomExceptionMiddleWare>();
            services.AddHttpClient();

            string serviceName = configuration.GetSection("FileStorage:ServiceName").Value;

            if (string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentNullException("FileStorage:ServiceName未配置");

            services.Configure<FileStorageOption>(configuration.GetSection("FileStorage"));

            if (serviceName == LinFile.LocalFileService)
            {
                services.AddTransient<IFileService, LocalFileService>();
            }
            else
            {
                services.AddTransient<IFileService, QiniuService>();
            }
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
            //注入计数器和规则存储
            services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();
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
                    default:
                        break;
                }

            }
            else
            {
                Log.Error($"CAP配置CAP:DefaultStorage:{defaultStorage.Value}无效");
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

    }
}
