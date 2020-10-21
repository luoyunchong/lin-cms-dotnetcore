﻿using System;
using System.Diagnostics;
using System.Threading;
using AspNetCoreRateLimit;
using CSRedis;
using DotNetCore.CAP;
using DotNetCore.CAP.Messages;
using DotNetCore.Security;
using FreeSql;
using FreeSql.Internal;
using IdentityServer4.Services;
using LinCms.Cms.Account;
using LinCms.Cms.Files;
using LinCms.Cms.Users;
using LinCms.Data.Authorization;
using LinCms.Data.Enums;
using LinCms.Data.Options;
using LinCms.Entities;
using LinCms.FreeSql;
using LinCms.Security;
using LinCms.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Savorboard.CAP.InMemoryMessageQueue;
using Serilog;
using ToolGood.Words;

namespace LinCms.Startup
{
    public static class DependencyInjectionExtensions
    {


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
            services.Configure<FileStorageOption>(configuration.GetSection("FileStorage"));
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
