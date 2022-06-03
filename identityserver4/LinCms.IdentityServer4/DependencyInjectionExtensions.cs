using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using DotNetCore.Security;
using FreeSql;
using FreeSql.Internal;
using IdentityServer4.Configuration;
using LinCms.Aop.Filter;
using LinCms.Cms.Users;
using LinCms.Data;
using LinCms.Data.Enums;
using LinCms.Entities;
using LinCms.Extensions;
using LinCms.FreeSql;
using LinCms.IdentityServer4.IdentityServer4;
using LinCms.IRepositories;
using LinCms.Repositories;
using LinCms.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Serilog;
#if !DEBUG
using System.Security.Cryptography.X509Certificates;
#endif
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

        #region AddServices
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration Configuration)
        {

            services.AddContext();

            services.AddCors();
            services.AddHashService();
            services.AddICryptographyService("lin-cms-dotnetcore-cryptography");

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/account/login";
            });
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(120);
            });

            services.AddIdentityServer(options => new IdentityServerOptions
            {
                UserInteraction = new UserInteractionOptions
                {
                    LoginUrl = "/account/login",
                    LogoutUrl = "/account/logout",
                }
            })
#if DEBUG
                .AddDeveloperSigningCredential()
#endif
#if !DEBUG
                .AddSigningCredential(new X509Certificate2(
                    Path.Combine(AppContext.BaseDirectory, Configuration["Certificates:Path"]),
                    Configuration["Certificates:Password"])
                )
#endif
                .AddInMemoryIdentityResources(InMemoryConfiguration.GetIdentityResources())
                .AddInMemoryApiResources(InMemoryConfiguration.GetApis())
                .AddInMemoryClients(InMemoryConfiguration.GetClients())
                .AddInMemoryApiScopes(InMemoryConfiguration.GetApiScopes())
                .AddProfileService<LinCmsProfileService>()
                .AddResourceOwnerValidator<LinCmsResourceOwnerPasswordValidator>();

            #region Swagger

            //Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(options =>
            {
                string ApiName = "LinCms.IdentityServer4";
                options.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = ApiName + RuntimeInformation.FrameworkDescription,
                    Version = "v1",
                    Contact = new OpenApiContact { Name = ApiName, Email = "luoyunchong@foxmail.com", Url = new Uri("https://www.cnblogs.com/igeekfan/") },
                    License = new OpenApiLicense { Name = ApiName + " 官方文档", Url = new Uri("https://luoyunchong.github.io/igeekfan-docs/dotnetcore/lin-cms/dotnetcore-start.html") }
                });
                var security = new OpenApiSecurityRequirement()
                {
                    { new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference()
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    }, Array.Empty<string>() }
                };
                options.AddSecurityRequirement(security);//添加一个必须的全局安全信息，和AddSecurityDefinition方法指定的方案名称要一致，这里是Bearer。
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 参数结构: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",//jwt默认的参数名称
                    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey
                });
                try
                {
                    string xmlPath = Path.Combine(AppContext.BaseDirectory, $"{typeof(Program).Assembly.GetName().Name}.xml");
                    options.IncludeXmlComments(xmlPath, true);
                }
                catch (Exception ex)
                {
                    Log.Logger.Warning(ex.Message);
                }
                options.AddServer(new OpenApiServer()
                {
                    Url = "",
                    Description = "vvv"
                });
                options.CustomOperationIds(apiDesc =>
                {
                    var controllerAction = apiDesc.ActionDescriptor as ControllerActionDescriptor;
                    return controllerAction.ControllerName + "-" + controllerAction.ActionName;
                });
            });
            #endregion

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserIdentityService, UserIdentityService>();
            services.AddTransient<ICurrentUser, CurrentUser>();
            services.AddTransient(typeof(IAuditBaseRepository<>), typeof(AuditBaseRepository<>));
            services.AddTransient(typeof(IAuditBaseRepository<,>), typeof(AuditBaseRepository<,>));
            //services.AddTransient<CustomExceptionMiddleWare>();

            services.AddAutoMapper(typeof(UserProfile).Assembly);

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add<LinCmsExceptionFilter>();
            })
            .AddNewtonsoftJson(opt =>
            {
                //忽略循环引用
                opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //opt.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:MM:ss";
                //设置自定义时间戳格式
                opt.SerializerSettings.Converters = new List<JsonConverter>()
                {
                    new LinCmsTimeConverter()
                };
                // 设置下划线方式，首字母是小写
                //opt.SerializerSettings.ContractResolver = new DefaultContractResolver()
                //{
                //    NamingStrategy = new SnakeCaseNamingStrategy()
                //    {
                //        ProcessDictionaryKeys = true
                //    }
                //};
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressConsumesConstraintForFormFileParameters = true;
                //自定义 BadRequest 响应
                options.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetails = new ValidationProblemDetails(context.ModelState);

                    var resultDto = new UnifyResponseDto(ErrorCode.ParameterError, problemDetails.Errors, context.HttpContext);

                    return new BadRequestObjectResult(resultDto)
                    {
                        ContentTypes = { "application/json" }
                    };
                };
            });
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
                options.HttpsPort = 443;
            });
            services.AddHealthChecks();

            return services;
        }
        #endregion

    }
}
