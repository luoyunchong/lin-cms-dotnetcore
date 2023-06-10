using System;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Entities;
using LinCms.FreeSql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace LinCms.Startup;

public static class ServiceProviderExtensions
{
    #region 获取一下Scope Service 以执行 IP Limit的 Redis初始化
    public static async Task RunScopeClientPolicy(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
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
    #endregion

    #region FreeSql执行同步结构SyncStructure
    public static IServiceProvider RunFreeSqlSyncStructure(this IServiceProvider serviceProvider)
    {
        using IServiceScope serviceScope = serviceProvider.CreateScope();
        var fsql = serviceScope.ServiceProvider.GetRequiredService<IFreeSql>();

        try
        {
            using var objPool = fsql.Ado.MasterPool.Get();
        }
        catch (Exception e)
        {
            Log.Error($"Message:{e.Message},StackTrace:{e.StackTrace}");
        }
        //在运行时直接生成表结构,初始化数据
        try
        {
            fsql.CodeFirst
                .SeedData()
                .SyncStructure(ReflexHelper.GetTypesByTableAttribute(typeof(LinUser)));
        }
        catch (Exception e)
        {
            Log.Error($"Message:{e.Message},StackTrace:{e.StackTrace}");
        }

        return serviceProvider;
    }
    #endregion
}