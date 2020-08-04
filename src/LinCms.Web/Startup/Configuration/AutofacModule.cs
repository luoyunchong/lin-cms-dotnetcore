using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extras.DynamicProxy;
using LinCms;
using LinCms.Cms.Account;
using LinCms.Cms.Files;
using LinCms.Cms.Users;
using LinCms.Data;
using LinCms.Data.Authorization;
using LinCms.Dependency;
using LinCms.Entities;
using LinCms.IRepositories;
using LinCms.Middleware;
using LinCms.Repositories;
using LinCms.Startup;
using LinCms.Startup.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace LinCms.Startup.Configuration
{
    public class AutofacModule : Autofac.Module
    {
        private readonly IConfiguration _configuration;
        public AutofacModule(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            string serviceName = _configuration.GetSection("FileStorage:ServiceName").Value;

            if (string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentNullException("FileStorage:ServiceName未配置");

            if (serviceName == LinFile.LocalFileService)
            {
                builder.RegisterType<LocalFileService>().As<IFileService>().InstancePerLifetimeScope();
            }
            else
            {
                builder.RegisterType<QiniuService>().As<IFileService>().InstancePerLifetimeScope();
            }

            bool isIds4 = _configuration.GetSection("Service:IdentityServer4").Value.ToBoolean();
            if (isIds4)
            {
                builder.RegisterType<IdentityServer4Service>().As<ITokenService>().InstancePerLifetimeScope();
            }
            else
            {
                builder.RegisterType<JwtTokenService>().As<ITokenService>().InstancePerLifetimeScope();
            }

            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
            builder.RegisterType<PermissionAuthorizationHandler>().As<IAuthorizationHandler>().InstancePerLifetimeScope();

            builder.RegisterType<MigrationStartupTask>().SingleInstance();
            builder.RegisterBuildCallback(async (c) => await c.Resolve<MigrationStartupTask>().StartAsync());

        }
    }

}
