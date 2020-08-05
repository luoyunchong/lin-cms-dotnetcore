using Autofac;
using Autofac.Extras.DynamicProxy;
using LinCms.Cms.Account;
using LinCms.Cms.Files;
using LinCms.Cms.Users;
using LinCms.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LinCms.Startup.Configuration
{
    public class ServiceModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UnitOfWorkInterceptor>();
            builder.RegisterType<UnitOfWorkAsyncInterceptor>();

            List<Type> interceptorServiceTypes = new List<Type>();
            interceptorServiceTypes.Add(typeof(UnitOfWorkInterceptor));

            string[] notIncludes = new string[]
            {
                typeof(QiniuService).Name,
                typeof(LocalFileService).Name,
                typeof(IdentityServer4Service).Name,
                typeof(JwtTokenService).Name
            };

            Assembly servicesDllFile = Assembly.Load("LinCms.Application");
            builder.RegisterAssemblyTypes(servicesDllFile)
                .Where(a => a.Name.EndsWith("Service") && !notIncludes.Where(r => r == a.Name).Any())
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .PropertiesAutowired()// 属性注入
                .InterceptedBy(interceptorServiceTypes.ToArray())
                .EnableInterfaceInterceptors();
        }
    }
}
