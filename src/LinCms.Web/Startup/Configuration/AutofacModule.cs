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
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
            builder.RegisterType<PermissionAuthorizationHandler>().As<IAuthorizationHandler>().InstancePerLifetimeScope();

            builder.RegisterType<MigrationStartupTask>().SingleInstance();
            builder.RegisterBuildCallback(async (c) => await c.Resolve<MigrationStartupTask>().StartAsync());

        }
    }

}
