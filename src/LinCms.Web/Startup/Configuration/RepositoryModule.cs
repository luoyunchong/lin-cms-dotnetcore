using System.Reflection;
using Autofac;

namespace LinCms.Startup.Configuration;

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
    }
}