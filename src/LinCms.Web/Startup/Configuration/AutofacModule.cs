using Autofac;
using LinCms.Data;
using LinCms.Data.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace LinCms.Startup.Configuration;

public class AutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<PermissionAuthorizationHandler>().As<IAuthorizationHandler>().InstancePerLifetimeScope();
        builder.RegisterType<ValidJtiHandler>().As<IAuthorizationHandler>().InstancePerLifetimeScope();

        //初始化种子数据
        builder.RegisterType<MigrationStartupTask>().SingleInstance();
        builder.RegisterBuildCallback(async (c) => await c.Resolve<MigrationStartupTask>().StartAsync());
    }
}