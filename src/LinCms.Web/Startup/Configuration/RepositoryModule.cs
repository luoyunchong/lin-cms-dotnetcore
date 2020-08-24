using Autofac;
using Autofac.Extras.DynamicProxy;
using LinCms.Cms.Account;
using LinCms.Cms.Files;
using LinCms.Cms.Users;
using LinCms.IRepositories;
using LinCms.Middleware;
using LinCms.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LinCms.Startup.Configuration
{

    /// <summary>
    /// 注入仓储接口
    /// </summary>
    public class RepositoryModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            Assembly assemblysRepository = Assembly.Load("LinCms.Infrastructure");
            builder.RegisterAssemblyTypes(assemblysRepository)
                    .Where(a => a.Name.EndsWith("Repository"))
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(AuditBaseRepository<>)).As(typeof(IAuditBaseRepository<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(AuditBaseRepository<,>)).As(typeof(IAuditBaseRepository<,>)).InstancePerLifetimeScope();

        }
    }
}
