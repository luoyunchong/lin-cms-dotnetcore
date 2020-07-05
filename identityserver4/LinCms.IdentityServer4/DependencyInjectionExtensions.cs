using System;
using System.Diagnostics;
using FreeSql;
using FreeSql.Internal;
using LinCms.Core.Entities;
using LinCms.Infrastructure.FreeSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinCms.IdentityServer4
{
    public static class DependencyInjectionExtensions
    {
        #region FreeSql
        /// <summary>
        /// FreeSql
        /// </summary>
        /// <param name="services"></param>
        public static void AddContext(this IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

            IFreeSql fsql = new FreeSqlBuilder()
                   .UseConnectionString(configuration)
                   .UseNameConvert(NameConvertType.PascalCaseToUnderscoreWithLower)
                   .UseAutoSyncStructure(true)
                   .UseMonitorCommand(cmd =>
                       {
                           Trace.WriteLine(cmd.CommandText);
                       }
                   )
                   .Build();
            fsql.CodeFirst.IsSyncStructureToLower = true;

            services.AddSingleton(fsql);
            services.AddScoped<UnitOfWorkManager>();
            services.AddFreeRepository(filter =>
            {
                filter.Apply<IDeleteAduitEntity>("IsDeleted", a => a.IsDeleted == false);
            });
        }
        #endregion
    }
}
