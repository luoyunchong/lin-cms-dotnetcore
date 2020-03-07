using System.Diagnostics;
using FreeSql;
using FreeSql.Internal;
using LinCms.Core.Entities;
using LinCms.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinCms.IdentityServer4
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
                   .UseEntityPropertyNameConvert(StringConvertType.PascalCaseToUnderscoreWithLower)
                   .UseAutoSyncStructure(true)
                   .UseMonitorCommand(cmd =>
                       {
                           Trace.WriteLine(cmd.CommandText);
                       }
                   )
                   .UseSyncStructureToLower(true) // 转小写同步结构
                   .Build();
            services.AddSingleton(fsql);
            services.AddScoped<IUnitOfWork>(sp => sp.GetService<IFreeSql>().CreateUnitOfWork());

            services.AddFreeRepository(filter =>
            {
                filter.Apply<IDeleteAduitEntity>("IsDeleted", a => a.IsDeleted == false);
            }, typeof(AuditBaseRepository<>).Assembly);
        }
        #endregion
    }
}
