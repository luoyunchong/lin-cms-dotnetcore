using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using FreeSql;
using LinCms.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace LinCms.FreeSql
{
    public static class LinCmsFreeSqlModules
    {
        public static void UseFreeSql(this IServiceCollection services)
        {
            var fsql = new FreeSqlBuilder()
                .UseConnectionString(DataType.MySql, @"Data Source=.;Database=LinCms;User ID=root;Password=123456;pooling=true;CharSet=utf8;port=3306;")
                .UseAutoSyncStructure(true) //自动迁移实体的结构到数据库
                .Build();

            services.AddSingleton<IFreeSql>(fsql);
            services.AddFreeRepository(filter => filter
                    .Apply<ISoftDeleteAduitEntity>("SoftDelete", a => a.IsDeleted == false)
                //.Apply<ITenant>("Tenant", a => a.TenantId == 1)
            );
        }
    }
}
