using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using LinCms.Data.Options;
using LinCms.Entities;
using LinCms.Exceptions;
using LinCms.SnakeCaseQuery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Serilog;

namespace LinCms.Startup
{
    public static class SwaggerExtensions
    {
        #region AddSwaggerGen
        /// <summary>
        /// Swagger 扩展方法配置
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="LinCmsException"></exception>
        public static IServiceCollection AddSwaggerGen(this IServiceCollection services)
        {
            //解决  https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1349#issuecomment-572537295
            services.AddSwaggerGenNewtonsoftSupport();

            SiteOption? siteOption = services.BuildServiceProvider().GetService<IOptionsSnapshot<SiteOption>>()?.Value;
            if (siteOption == null)
            {
                throw new LinCmsException("you should add  `services.Configure<SiteOption>(configuration.GetSection(\"Site\"));` before get this options value ");
            }

            //Swagger重写PascalCase，改成SnakeCase模式
            services.TryAddEnumerable(ServiceDescriptor.Transient<IApiDescriptionProvider, ApiDescriptionProvider>());

            //Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(options =>
            {
                string ApiName = "LinCms.Web";
                options.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = ApiName + RuntimeInformation.FrameworkDescription,
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = ApiName,
                        Email = siteOption.Email,
                        Url = new Uri(siteOption.BlogUrl)
                    },
                    License = new OpenApiLicense
                    {
                        Name = ApiName + " 官方文档",
                        Url = new Uri(siteOption.DocUrl)
                    },
                });

                options.SwaggerDoc("cms", new OpenApiInfo() { Title = ApiName + RuntimeInformation.FrameworkDescription, Version = "cms" });
                options.SwaggerDoc("base", new OpenApiInfo() { Title = ApiName + RuntimeInformation.FrameworkDescription, Version = "base" });
                options.SwaggerDoc("blog", new OpenApiInfo() { Title = ApiName + RuntimeInformation.FrameworkDescription, Version = "blog" });

                //添加一个必须的全局安全信息，和AddSecurityDefinition方法指定的方案名称要一致，这里是Bearer。
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference()
                            {
                                Id =  "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                });
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 参数结构: \"Authorization: Bearer {token}\"",
                    Name = "Authorization", //jwt默认的参数名称
                    In = ParameterLocation.Header, //jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey

                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference()
                            {
                                Id =  "oauth2",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                });
                // Define the OAuth2.0 scheme that's in use (i.e. Implicit Flow)
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = new Uri(siteOption.IdentityServer4Domain + "/connect/authorize", UriKind.Absolute),
                            TokenUrl = new Uri(siteOption.IdentityServer4Domain + "/connect/token", UriKind.Absolute),
                            Scopes = new Dictionary<string, string>
                            {
                                { "LinCms.Web", "Access read/write LinCms.Web" }
                            }
                        },
                        Password = new OpenApiOAuthFlow()
                        {
                            AuthorizationUrl = new Uri(siteOption.IdentityServer4Domain + "/connect/authorize", UriKind.Absolute),
                            TokenUrl = new Uri(siteOption.IdentityServer4Domain + "/connect/token", UriKind.Absolute),
                            Scopes = new Dictionary<string, string>
                            {
                                { "openid", "Access read openid" },
                                { "offline_access", "Access read offline_access" },
                                { "LinCms.Web", "Access read/write LinCms.Web" }
                            }
                        }
                    },
                    Extensions = new Dictionary<string, IOpenApiExtension>()
                    {
                       {"x-client-id", new OpenApiString("lin-cms-dotnetcore-client-id")},
                       {"x-client-secret", new OpenApiString("lin-cms-dotnetcore-client-secrets")},
                    }
                });

                try
                {
                    string xmlPath = Path.Combine(AppContext.BaseDirectory, $"{typeof(Program).Assembly.GetName().Name}.xml");
                    options.IncludeXmlComments(xmlPath, true);
                    //实体层的xml文件名
                    string xmlEntityPath = Path.Combine(AppContext.BaseDirectory, $"{typeof(IEntity).Assembly.GetName().Name}.xml");
                    options.IncludeXmlComments(xmlEntityPath);
                    //Dto所在类库
                    string applicationPath = Path.Combine(AppContext.BaseDirectory, $"{typeof(IApplicationService).Assembly.GetName().Name}.xml");
                    options.IncludeXmlComments(applicationPath);
                }
                catch (Exception ex)
                {
                    Log.Logger.Warning(ex.Message);
                }

                options.AddServer(new OpenApiServer()
                {
                    Url = "https://localhost:5001",
                    Description = "本地"
                }); ;
                options.AddServer(new OpenApiServer()
                {
                    Url = "https://api.igeekfan.cn",
                    Description = "服务端"
                });

                options.CustomOperationIds(apiDesc =>
                {
                    var controllerAction = apiDesc.ActionDescriptor as ControllerActionDescriptor;
                    if (controllerAction == null) return Guid.NewGuid().ToString();
                    return $"{controllerAction.ControllerName}-{controllerAction.ActionName}";//-{controllerAction.GetHashCode()}
                });
            });
            return services;
        }
        #endregion
    }
}
