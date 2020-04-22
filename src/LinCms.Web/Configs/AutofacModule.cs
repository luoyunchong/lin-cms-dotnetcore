using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using LinCms.Core.Dependency;
using LinCms.Core.IRepositories;
using LinCms.Infrastructure.Repositories;
using LinCms.Web.Data;
using LinCms.Web.Data.Authorization;
using LinCms.Web.Uow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace LinCms.Web.Configs
{
    public class AutofacModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            Assembly servicesDllFile = Assembly.Load("LinCms.Application");
            Assembly assemblysRepository = Assembly.Load("LinCms.Infrastructure");

            var interceptorServiceTypes = new List<Type>();
            
            //builder.RegisterType<UnitOfWorkInterceptor>();
            //interceptorServiceTypes.Add(typeof(UnitOfWorkInterceptor));

            builder.RegisterAssemblyTypes(servicesDllFile).Where(a => a.Name.EndsWith("Service"))
                      .AsImplementedInterfaces()
                      .InstancePerLifetimeScope()
                      .EnableInterfaceInterceptors()
                      .InterceptedBy(interceptorServiceTypes.ToArray());

            builder.RegisterAssemblyTypes(assemblysRepository).Where(a => a.Name.EndsWith("Repository"))
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(AuditBaseRepository<>)).As(typeof(IAuditBaseRepository<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(AuditBaseRepository<,>)).As(typeof(IAuditBaseRepository<,>)).InstancePerLifetimeScope();

            Assembly[] currentAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(r => r.FullName.Contains("LinCms.")).ToArray();

            //每次调用，都会重新实例化对象；每次请求都创建一个新的对象；
            Type transientDependency = typeof(ITransientDependency);
            builder.RegisterAssemblyTypes(currentAssemblies)
                .Where(t => transientDependency.GetTypeInfo().IsAssignableFrom(t) && t.IsClass&&!t.IsAbstract&&!t.IsGenericType)
                .AsImplementedInterfaces().InstancePerDependency();

            //同一个Lifetime生成的对象是同一个实例
            Type scopeDependency = typeof(IScopedDependency);
            builder.RegisterAssemblyTypes(currentAssemblies)
                .Where(t => scopeDependency.GetTypeInfo().IsAssignableFrom(t) && t.IsClass && !t.IsAbstract && !t.IsGenericType)
                .AsImplementedInterfaces().InstancePerLifetimeScope();

            //单例模式，每次调用，都会使用同一个实例化的对象；每次都用同一个对象；
            Type singletonDependency = typeof(ISingletonDependency);
            builder.RegisterAssemblyTypes(currentAssemblies)
                .Where(t => singletonDependency.GetTypeInfo().IsAssignableFrom(t) && t.IsClass && !t.IsAbstract && !t.IsGenericType)
                .AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<MigrationStartupTask>().SingleInstance().OnActivated(async args => await args.Instance.StartAsync());

        }
    }

}
