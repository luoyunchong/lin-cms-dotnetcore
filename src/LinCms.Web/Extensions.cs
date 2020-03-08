using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using CSRedis;
using FreeSql;
using FreeSql.Internal;
using LinCms.Application;
using LinCms.Core.Dependency;
using LinCms.Core.Entities;
using LinCms.Infrastructure.Repositories;
using LinCms.Web.Data.Authorization;
using LinCms.Web.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using ToolGood.Words;

namespace LinCms.Web
{
    public static class Extensions
    {
        #region FreeSql
        /// <summary>
        /// FreeSql
        /// </summary>
        /// <param name="services"></param>
        public static void AddContext(this IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            IConfigurationSection configurationSection = configuration.GetSection("ConnectionStrings:MySql");
            IFreeSql fsql = new FreeSqlBuilder()
                   .UseConnectionString(DataType.MySql, configurationSection.Value)
                   .UseEntityPropertyNameConvert(StringConvertType.PascalCaseToUnderscoreWithLower)//全局转换实体属性名方法 https://github.com/2881099/FreeSql/pull/60
                   .UseAutoSyncStructure(true) //自动迁移实体的结构到数据库
                   .UseMonitorCommand(cmd =>
                       {
                           Trace.WriteLine(cmd.CommandText+";");
                       }
                   )
                   .UseSyncStructureToLower(true) // 转小写同步结构
                   .Build()
                   .SetDbContextOptions(opt => opt.EnableAddOrUpdateNavigateList = true);//联级保存功能开启（默认为关闭）



            fsql.Aop.CurdBefore += (s, e) =>
            {

            };

            fsql.Aop.CurdAfter += (s, e) =>
            {
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
            services.AddScoped<IUnitOfWork>(sp => sp.GetService<IFreeSql>().CreateUnitOfWork());

            services.AddFreeRepository(filter =>
            {
                filter.Apply<IDeleteAduitEntity>("IsDeleted", a => a.IsDeleted == false);
            }, typeof(AuditBaseRepository<>).Assembly);
        }
        #endregion

        public static void AddCsRedisCore(this IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
            #region 初始化 Redis配置
            IConfigurationSection csRediSection = configuration.GetSection("ConnectionStrings:CsRedis");
            CSRedisClient csredis = new CSRedisClient(csRediSection.Value);
            //初始化 RedisHelper
            RedisHelper.Initialization(csredis);
            //注册mvc分布式缓存
            services.AddSingleton<IDistributedCache>(new CSRedisCache(RedisHelper.Instance));
            #endregion
        }

        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            #region Scrutor 批量注册 

            Assembly[] currentAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(r => r.FullName.Contains("LinCms.")).ToArray();
            services.Scan(scan => scan
                //加载IAppService这个类所在的程序集
                .FromAssemblyOf<IAppService>()
                // 表示要注册那些类，上面的代码还做了过滤，只留下了以 Service 结尾的类
                .AddClasses(classes => classes.Where(t => t.Name != "IFileService" && t.Name.EndsWith("Service", StringComparison.OrdinalIgnoreCase)))
                .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                //表示将类型注册为提供其所有公共接口作为服务
                .AsImplementedInterfaces()
                //表示注册的生命周期为 Scope
                .WithScopedLifetime()
                //扫描当前以LinCms.开头类库 并且继承ITransientDependency的接口以“瞬时”注入到容器中
                .FromAssemblies(currentAssemblies)
                .AddClasses(classes => classes.AssignableTo<ITransientDependency>())
                .AsImplementedInterfaces()
                .WithTransientLifetime()
                //继承IScopeDependency的接口注入到容器中以“范围（单次请求获取的是一个实例）注入到容器中
                .FromAssemblies(currentAssemblies)
                .AddClasses(classes => classes.AssignableTo<IScopedDependency>())
                .AsImplementedInterfaces()
                .WithScopedLifetime()

                //继承ISingletonDependency的接口以“单例”生命周期注入到容器中
                .FromAssemblies(currentAssemblies)
                .AddClasses(classes => classes.AssignableTo<ISingletonDependency>())
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
            );

            #endregion

            //将Handler注册到DI系统中
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        }
    }
}
