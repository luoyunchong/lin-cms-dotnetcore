using Autofac;
using Autofac.Extras.DynamicProxy;
using LinCms.Cms.Account;
using LinCms.Cms.Files;
using LinCms.Cms.Users;
using LinCms.Entities;
using LinCms.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LinCms.Startup.Configuration
{
    /// <summary>
    /// 注入Application层中的Service
    /// </summary>
    public class ServiceModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UnitOfWorkInterceptor>();
            builder.RegisterType<UnitOfWorkAsyncInterceptor>();

            builder.RegisterType<AopCacheIntercept>();

            List<Type> interceptorServiceTypes = new List<Type>()
            {
                typeof(UnitOfWorkInterceptor),
                typeof(AopCacheIntercept),
            };

            string[] notIncludes = new string[]
            {
                typeof(QiniuService).Name,
                typeof(LocalFileService).Name,
                typeof(IdentityServer4Service).Name,
                typeof(JwtTokenService).Name,
                typeof(GithubOAuth2Serivice).Name,
                typeof(GiteeOAuth2Service).Name,
                typeof(QQOAuth2Service).Name
            };

            Assembly servicesDllFile = Assembly.Load("LinCms.Application");
            builder.RegisterAssemblyTypes(servicesDllFile)
                .Where(a => a.Name.EndsWith("Service") && !notIncludes.Where(r => r == a.Name).Any() && !a.IsAbstract && !a.IsInterface && a.IsPublic)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope()
                .PropertiesAutowired()// 属性注入
                .InterceptedBy(interceptorServiceTypes.ToArray())
                .EnableInterfaceInterceptors();

            //太慢了。
            //Assembly webDllFile = Assembly.Load("LinCms.Web");
            //builder.RegisterAssemblyTypes(webDllFile)
            //    .Where(a => a.Name.EndsWith("Controller") && !a.IsAbstract && !a.IsInterface && a.IsPublic)
            //    .InstancePerLifetimeScope()
            //    .AsSelf()
            //    .PropertiesAutowired()// 属性注入
            //    .InterceptedBy(interceptorServiceTypes.ToArray())
            //    .EnableClassInterceptors();



            //一个接口多个实现，使用Named，区分
            builder.RegisterType<LocalFileService>().Named<IFileService>(LinFile.LocalFileService).InstancePerLifetimeScope();
            builder.RegisterType<QiniuService>().Named<IFileService>(LinFile.QiniuService).InstancePerLifetimeScope();

            builder.RegisterType<IdentityServer4Service>().Named<ITokenService>(typeof(IdentityServer4Service).Name).InstancePerLifetimeScope();
            builder.RegisterType<JwtTokenService>().Named<ITokenService>(typeof(JwtTokenService).Name).InstancePerLifetimeScope();

            builder.RegisterType<GithubOAuth2Serivice>().Named<IOAuth2Service>(LinUserIdentity.GitHub).InstancePerLifetimeScope();
            builder.RegisterType<GiteeOAuth2Service>().Named<IOAuth2Service>(LinUserIdentity.Gitee).InstancePerLifetimeScope();
            builder.RegisterType<QQOAuth2Service>().Named<IOAuth2Service>(LinUserIdentity.QQ).InstancePerLifetimeScope();

        }
    }
}
