using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using AspNetCoreRateLimit;
using CSRedis;
using DotNetCore.CAP;
using DotNetCore.CAP.Messages;
using FreeSql;
using FreeSql.Internal;
using IGeekFan.FreeKit.Email;
using IGeekFan.FreeKit.Extras.AuditEntity;
using IGeekFan.FreeKit.Extras.CaseQuery;
using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Aop.Filter;
using LinCms.Data;
using LinCms.Data.Enums;
using LinCms.Data.Options;
using LinCms.Entities;
using LinCms.Extensions;
using LinCms.FreeSql;
using LinCms.Middleware;
using LinCms.Models.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owl.reCAPTCHA;
using Savorboard.CAP.InMemoryMessageQueue;
using Serilog;
using Yitter.IdGenerator;

namespace LinCms.Startup;

public static class ServiceCollectionExtensions
{
    #region Add 一些服务

    /// <summary>
    /// 一些需要的服务，配置信息
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddLinServices(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddTransient<CustomExceptionMiddleWare>();

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });


        //应用程序级别设置
        services.Configure<FormOptions>(options =>
        {
            //单个文件上传的大小限制为8 MB      默认134217728 应该是128MB
            options.MultipartBodyLengthLimit = 1024 * 1024 * 8; //8MB
        });

        services.AddHttpClient("IdentityServer4");
        services.AddEmailSender(configuration);
        services.Configure<LoginCaptchaOption>(configuration.GetSection("LoginCaptcha"));
        services.Configure<FileStorageOption>(configuration.GetSection("FileStorage"));
        services.Configure<SiteOption>(configuration.GetSection("Site"));
        return services;
    }

    #endregion

    #region AddCustomMvc

    public static IServiceCollection AddCustomMvc(this IServiceCollection services, IConfiguration c)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddMvc(options =>
            {
                options.ValueProviderFactories.Add(new SnakeCaseValueProviderFactory()); //设置SnakeCase形式的QueryString参数
                options.Filters.Add<UnitOfWorkActionFilter>();
                options.Filters.Add<AopCacheableActionFilter>();
                options.Filters.Add<LogActionFilterAttribute>(); // 添加请求方法时的日志记录过滤器
                options.Filters.Add<LinCmsExceptionFilter>(); // 
            })
            .AddNewtonsoftJson(opt =>
            {
                opt.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                // 设置自定义时间戳格式
                opt.SerializerSettings.Converters = new List<JsonConverter>()
                {
                    new LinCmsTimeConverter()
                };
                // 设置下划线方式，首字母是小写
                opt.SerializerSettings.ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                    {
                        //ProcessDictionaryKeys = true
                    },
                };
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                //options.SuppressConsumesConstraintForFormFileParameters = true; //SuppressUseValidationProblemDetailsForInvalidModelStateResponses;
                //自定义 BadRequest 响应
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState);
                    var resultDto = new UnifyResponseDto(ErrorCode.ParameterError, problemDetails.Errors,
                        context.HttpContext);

                    return new BadRequestObjectResult(resultDto)
                    {
                        ContentTypes = { "application/json" }
                    };
                };
            });

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder
                    .SetIsOriginAllowed((host) => true)
                    //.WithOrigins(c.GetSection("WithOrigins").Get<string[]>())
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
        });

        return services;
    }

    #endregion

    #region FreeSql

    /// <summary>
    /// FreeSql
    /// </summary>
    /// <param name="services"></param>
    /// <param name="c"></param>
    public static IServiceCollection AddFreeSql(this IServiceCollection services, IConfiguration c)
    {
        var options = new IdGeneratorOptions(1);
        // options.WorkerIdBitLength = 10; // 默认值6，限定 WorkerId 最大值为2^6-1，即默认最多支持64个节点。
        // options.SeqBitLength = 6; // 默认值6，限制每毫秒生成的ID个数。若生成速度超过5万个/秒，建议加大 SeqBitLength 到 10。
        // options.BaseTime = Your_Base_Time; // 如果要兼容老系统的雪花算法，此处应设置为老系统的BaseTime。
        // ...... 其它参数参考 IdGeneratorOptions 定义。
        YitIdHelper.SetIdGenerator(options);
        // 保存参数（务必调用，否则参数设置不生效）

        Func<IServiceProvider, IFreeSql> fsql = r =>
        {
            IFreeSql fsql = new FreeSqlBuilder()
                .UseConnectionString(c)
                .UseMappingPriority(MappingPriorityType.Aop, MappingPriorityType.FluentApi, MappingPriorityType.Attribute)
                .UseNameConvert(NameConvertType.PascalCaseToUnderscoreWithLower)
                .UseAutoSyncStructure(true)
                .UseNoneCommandParameter(true)
                .CreateDatabaseIfNotExists()
                .Build()
                .SetDbContextOptions(opt => opt.EnableCascadeSave = true); //联级保存功能开启（默认为关闭）

            //以解决首次运行时，app_serilog未初始化的问题：必须保证数据库创建
            #region Serilog日志
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(c).Enrich.FromLogContext().CreateLogger();
            Log.Information("Starting web host");
#if DEBUG
            Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
#endif
            #endregion

            #region Message Template
            string messageTemplate = @"
--------------------------BEGIN----------------------------------------------
Sql:{0}
CurrentThread ManagedThreadId:{1}
EntityType FullName:{2}
ElapsedMilliseconds:{3}ms
--------------------------END------------------------------------------------
";
            fsql.Aop.CurdAfter += (s, e) =>
            {
                var sqlInfo = string.Format(messageTemplate, e.Sql, Thread.CurrentThread.ManagedThreadId, e.EntityType.FullName, e.ElapsedMilliseconds);

                Log.Information(sqlInfo);

                if (e.ElapsedMilliseconds > 200)
                {
                    //记录日志
                    //发送短信给负责人
                }
            };
            #endregion

            fsql.GlobalFilter.Apply<ISoftDelete>("IsDeleted", a => a.IsDeleted == false);

            fsql.CodeFirst.ConfigEntity<LinUser>(r =>
            {
                r.Property(b => b.Id).IsIdentity(false);
            });

            fsql.Aop.AuditValue += (s, e) =>
            {
                if (e.Column.CsType == typeof(long) && e.Property.Name == "Id" && e.Value?.ToString() == "0")
                {
                    e.Value = YitIdHelper.NextId();
                    //e.Column.Attribute.IsIdentity = false;
                }
            };
            fsql.Aop.ConfigEntityProperty += (s, e) =>
            {
                if (e.Property.Name == "Id")
                    e.ModifyResult.IsIdentity = false;
            };
            //数据库特性 > 实体特性 > FluentApi（配置特性） > Aop（配置特性）

            //敏感词处理
            if (c["AuditValue:Enable"].ToBoolean())
            {
                //已过期，开发者不维护
                //IllegalWordsSearch illegalWords = ToolGoodUtils.GetIllegalWordsSearch();

                //fsql.Aop.AuditValue += (s, e) =>
                //{
                //    if (e.Column.CsType == typeof(string) && e.Value != null)
                //    {
                //        string oldVal = (string)e.Value;
                //        string newVal = illegalWords.Replace(oldVal);
                //        //第二种处理敏感词的方式
                //        //string newVal = oldVal.ReplaceStopWords();
                //        if (newVal != oldVal)
                //        {
                //            e.Value = newVal;
                //        }
                //    }
                //};
            }

            fsql.UseJsonMap();
            return fsql;
        };

        services.AddSingleton(fsql);

        services.AddFreeKitCore(typeof(long));

        services.Configure<UnitOfWorkDefaultOptions>(u =>
        {
            u.IsolationLevel = System.Data.IsolationLevel.ReadCommitted;
            u.Propagation = Propagation.Required;
        });
        return services;
    }

    #endregion

    #region 初始化 Redis配置

    public static IServiceCollection AddCsRedisCore(this IServiceCollection services, IConfiguration c)
    {
        //初始化 RedisHelper
        RedisHelper.Initialization(new CSRedisClient(c.GetConnectionString("CsRedis")));
        //注册mvc分布式缓存
        services.AddSingleton<IDistributedCache>(r => new CSRedisCache(RedisHelper.Instance));
        return services;
    }

    #endregion

    #region 配置限流依赖的服务

    /// <summary>
    /// 配置限流依赖的服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddIpRateLimiting(this IServiceCollection services,
        IConfiguration configuration)
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

    #endregion

    #region 分布式事务一致性CAP

    public static CapOptions UseCapOptions(this CapOptions @this, IConfiguration c)
    {
        IConfigurationSection defaultStorage = c.GetSection("CAP:DefaultStorage");
        IConfigurationSection defaultMessageQueue = c.GetSection("CAP:DefaultMessageQueue");
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
                    IConfigurationSection mySql = c.GetSection($"ConnectionStrings:MySql");
                    @this.UseMySql(mySql.Value);
                    break;
                //case CapStorageType.SqlServer:
                //    IConfigurationSection sqlServer = c.GetSection($"ConnectionStrings:SqlServer");
                //    @this.UseSqlServer(opt =>
                //    {
                //        opt.ConnectionString = sqlServer.Value;
                //        //使用SQL SERVER2008才需要打开他
                //        //opt.UseSqlServer2008();
                //    });
                //    break;
                default:
                    break;
            }
        }
        else
        {
            Log.Error(
                $"CAP:DefaultStorage:{capStorageType}配置无效，仅支持InMemoryStorage，Mysql！更多请增加引用，修改配置项代码");
        }

        if (Enum.TryParse(defaultMessageQueue.Value, out CapMessageQueueType capMessageQueueType))
        {
            if (!Enum.IsDefined(typeof(CapMessageQueueType), capMessageQueueType))
            {
                Log.Error($"CAP配置CAP:DefaultMessageQueue:{defaultMessageQueue.Value}无效");
            }

            IConfigurationSection configurationSection = c.GetSection($"ConnectionStrings:{capMessageQueueType}");

            switch (capMessageQueueType)
            {
                case CapMessageQueueType.InMemoryQueue:
                    @this.UseInMemoryMessageQueue();
                    break;
                case CapMessageQueueType.RabbitMQ:
                    @this.UseRabbitMQ(options =>
                    {
                        options.HostName = c["CAP:RabbitMQ:HostName"];
                        options.UserName = c["CAP:RabbitMQ:UserName"];
                        options.Password = c["CAP:RabbitMQ:Password"];
                        options.VirtualHost = c["CAP:RabbitMQ:VirtualHost"];
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

    public static IServiceCollection AddCap(this IServiceCollection services, IConfiguration c)
    {
        services.AddCap(x =>
        {
            try
            {
                x.UseCapOptions(c);
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
                Log.Error(
                    $@"A message of type {type} failed after executing {x.FailedRetryCount} several times, requiring manual troubleshooting. Message name: {type.Message.GetName()}");
            };
        });

        return services;
    }

    #endregion

    #region 配置Google验证码

    public static IServiceCollection AddGooglereCaptchav3(this IServiceCollection services, IConfiguration c)
    {
        services.AddScoped<RecaptchaVerifyActionFilter>();
        services.Configure<GooglereCAPTCHAOptions>(c.GetSection(GooglereCAPTCHAOptions.RecaptchaSettings));
        GooglereCAPTCHAOptions googlereCaptchaOptions = new GooglereCAPTCHAOptions();
        c.GetSection(GooglereCAPTCHAOptions.RecaptchaSettings).Bind(googlereCaptchaOptions);
        services.AddreCAPTCHAV3(x =>
        {
            x.VerifyBaseUrl = googlereCaptchaOptions.VerifyBaseUrl;
            x.SiteKey = googlereCaptchaOptions.SiteKey;
            x.SiteSecret = googlereCaptchaOptions.SiteSecret;
        });
        return services;
    }

    #endregion
}