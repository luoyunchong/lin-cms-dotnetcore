using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AutoMapper;
using FreeSql;
using FreeSql.Internal;
using IdentityServer4.AccessTokenValidation;
using LinCms.Plugins.Poem.AutoMapper;
using LinCms.Web.Data;
using LinCms.Web.Data.Authorization;
using LinCms.Web.Data.IdentityServer4;
using LinCms.Web.Middleware;
using LinCms.Web.Services.Cms;
using LinCms.Web.Services.Cms.Interfaces;
using LinCms.Web.Utils;
using LinCms.Zero.Aop;
using LinCms.Zero.Data;
using LinCms.Zero.Data.Enums;
using LinCms.Zero.Data.Oauth2;
using LinCms.Zero.Dependency;
using LinCms.Zero.Domain;
using LinCms.Zero.Extensions;
using LinCms.Zero.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Scrutor;
using ToolGood.Words;

namespace LinCms.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public static IFreeSql Fsql { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            IConfigurationSection configurationSection = Configuration.GetSection("ConnectionStrings:Default");

            Fsql = new FreeSqlBuilder()
                .UseConnectionString(DataType.MySql, configurationSection.Value)
                .UseEntityPropertyNameConvert(StringConvertType.PascalCaseToUnderscoreWithLower)//全局转换实体属性名方法 https://github.com/2881099/FreeSql/pull/60
                .UseAutoSyncStructure(true) //自动迁移实体的结构到数据库
                .UseMonitorCommand(cmd =>
                    {
                        Trace.WriteLine(cmd.CommandText);
                    }
                )
                .UseSyncStructureToLower(true) // 转小写同步结构
                .Build();

            Fsql.Aop.CurdBefore = (s, e) =>
            {

            };

            Fsql.Aop.CurdAfter = (s, e) =>
            {
                if (e.ElapsedMilliseconds > 200)
                {
                    //记录日志
                    //发送短信给负责人
                }
            };

            //敏感词处理
            IllegalWordsSearch illegalWords = ToolGoodUtils.GetIllegalWordsSearch();

            Fsql.Aop.AuditValue += (s, e) =>
            {
                if (e.Column.CsType == typeof(string) && e.Value != null)
                {
                    string oldVal = (string)e.Value;
                    string newVal = illegalWords.Replace(oldVal);
                    //第二种处理敏感词的方式
                    //string newVal = oldVal.ReplaceStopWords();
                    if (newVal != oldVal)
                    {
                        e.Value = newVal;
                    }
                }
            };
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            #region IdentityServer4+FreeSql
            InMemoryConfiguration.Configuration = this.Configuration;
            services.AddSingleton(Fsql);
            services.AddSingleton(typeof(IUnitOfWork), typeof(UnitOfWork));

            services.AddFreeRepository(filter =>
            {
                filter.Apply<IDeleteAduitEntity>("IsDeleted", a => a.IsDeleted == false);
            }, GetType().Assembly, typeof(AuditBaseRepository<>).Assembly);

            services.AddIdentityServer()
#if  DEBUG
                .AddDeveloperSigningCredential()
#endif
#if !DEBUG
                .AddSigningCredential(new X509Certificate2(Path.Combine(AppContext.BaseDirectory,
                        Configuration["Certificates:Path"]),
                    Configuration["Certificates:Password"]))
#endif
                .AddInMemoryIdentityResources(InMemoryConfiguration.GetIdentityResources())
                .AddInMemoryApiResources(InMemoryConfiguration.GetApis())
                .AddInMemoryClients(InMemoryConfiguration.GetClients())
                .AddProfileService<LinCmsProfileService>()
                .AddResourceOwnerValidator<LinCmsResourceOwnerPasswordValidator>();


            #region AddAuthentication\AddIdentityServerAuthentication 
            //AddAuthentication()是把验证服务注册到DI, 并配置了Bearer作为默认模式.

            //AddIdentityServerAuthentication()是在DI注册了token验证的处理者.
            //由于是本地运行, 所以就不使用https了, RequireHttpsMetadata = false.如果是生产环境, 一定要使用https.
            //Authority指定Authorization Server的地址.
            //ApiName要和Authorization Server里面配置ApiResource的name一样.
            //和  AddJwtBearer不能同时使用，目前还不理解区别。
            //services
            //    .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            //    .AddIdentityServerAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
            //    {
            //        options.RequireHttpsMetadata = false; // for dev env
            //        options.Authority = $"{Configuration["Identity:Protocol"]}://{Configuration["Identity:IP"]}:{Configuration["Identity:Port"]}"; ;
            //        options.ApiName = Configuration["Service:Name"]; // match with configuration in IdentityServer

            //        //options.JwtValidationClockSkew = TimeSpan.FromSeconds(60 * 5);

            //    });
            #endregion

            #region AddJwtBearer
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    //identityserver4 地址 也就是本项目地址
                    options.Authority = $"{Configuration["Identity:Protocol"]}://{Configuration["Identity:IP"]}:{Configuration["Identity:Port"]}";
                    options.RequireHttpsMetadata = false;
                    options.Audience = Configuration["Service:Name"];

                    //options.TokenValidationParameters = new TokenValidationParameters()
                    //{
                    //    ClockSkew = TimeSpan.Zero   //偏移设置为了0s,用于测试过期策略,完全按照access_token的过期时间策略，默认原本为5分钟
                    //};


                    //使用Authorize设置为需要登录时，返回json格式数据。
                    options.Events = new JwtBearerEvents()
                    {
                        OnChallenge = context =>
                        {
                            //此处代码为终止.Net Core默认的返回类型和数据结果，这个很重要哦
                            context.HandleResponse();

                            string msg;
                            ErrorCode errorCode;
                            int statusCode = StatusCodes.Status401Unauthorized;

                            if (context.Error == "invalid_token" &&
                               context.ErrorDescription == "The token is expired")
                            {
                                msg = "令牌过期";
                                errorCode = ErrorCode.TokenExpired;
                                statusCode = StatusCodes.Status422UnprocessableEntity;
                            }
                            else if (context.Error == "invalid_token" && context.ErrorDescription.IsNullOrEmpty())
                            {
                                msg = "令牌失效";
                                errorCode = ErrorCode.TokenInvalidation;
                            }

                            else
                            {
                                msg = "请先登录";//""认证失败，请检查请求头或者重新登录";
                                errorCode = ErrorCode.AuthenticationFailed;
                            }

                            context.Response.ContentType = "application/json";
                            context.Response.StatusCode = statusCode;
                            context.Response.WriteAsync(new ResultDto(errorCode, msg, context.HttpContext).ToString());

                            return Task.FromResult(0);
                        }
                    };
                })
                .AddGitHub(options =>
                {
                    options.ClientId = Configuration["OAuth2:GitHub:ClientId"];
                    options.ClientSecret = Configuration["OAuth2:GitHub:ClientSecret"];
                    options.Scope.Add("user:email");
                });
            #endregion

            #endregion

            services.AddAutoMapper(typeof(Startup).Assembly, typeof(PoemProfile).Assembly);

            services.Configure<OAuth2Options>(Configuration);

            //services.AddCors(option => option.AddPolicy("cors", policy => policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().AllowAnyOrigin()));
            services.AddCors();

            #region Mvc
            services.AddControllers(options =>
             {
                 options.ValueProviderFactories.Add(new SnakeCaseQueryValueProviderFactory());//设置SnakeCase形式的QueryString参数
                 //options.Filters.Add<LinCmsExceptionFilter>();
                 //options.Filters.Add<LogActionFilterAttribute>(); // 添加请求方法时的日志记录过滤器
             })
             .AddNewtonsoftJson(opt =>
             {
                 //opt.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:MM:ss";
                 //设置时间戳格式
                 opt.SerializerSettings.Converters = new List<JsonConverter>()
                 {
                        new LinCmsTimeConverter()
                 };
                 // 设置下划线方式，首字母是小写
                 opt.SerializerSettings.ContractResolver = new DefaultContractResolver()
                 {
                     NamingStrategy = new SnakeCaseNamingStrategy()
                     {
                         ProcessDictionaryKeys = true
                     }
                 };
             })
             .ConfigureApiBehaviorOptions(options =>
             {
                 options.SuppressConsumesConstraintForFormFileParameters = true;//SuppressUseValidationProblemDetailsForInvalidModelStateResponses;
                 //自定义 BadRequest 响应
                 options.InvalidModelStateResponseFactory = context =>
                 {
                     var problemDetails = new ValidationProblemDetails(context.ModelState);

                     var resultDto = new ResultDto(ErrorCode.ParameterError, problemDetails.Errors, context.HttpContext);

                     return new BadRequestObjectResult(resultDto)
                     {
                         ContentTypes = { "application/json" }
                     };
                 };
             });
            #endregion

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            #region Scrutor 与单个单个注册等价，不过可批量注册 
            //services.AddScoped<ILogService, LogService>();
            //services.AddScoped<IUserSevice, UserService>();
            services.Scan(scan => scan
                    //加载Startup这个类所在的程序集
                    .FromAssemblyOf<Startup>()
                    // 表示要注册那些类，上面的代码还做了过滤，只留下了以 Service 结尾的类
                    .AddClasses(classes => classes.Where(t => t.Name != "IFileService" && t.Name.EndsWith("Service", StringComparison.OrdinalIgnoreCase)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    //表示将类型注册为提供其所有公共接口作为服务
                    .AsImplementedInterfaces()
                    //表示注册的生命周期为 Transient
                    .WithTransientLifetime()
                    // We start out with all types in the assembly of ITransientService
                    .FromAssemblyOf<IScopeDependency>()
                    // AddClasses starts out with all public, non-abstract types in this assembly.
                    // These types are then filtered by the delegate passed to the method.
                    // In this case, we filter out only the classes that are assignable to ITransientService.
                    .AddClasses(classes => classes.AssignableTo<ITransientDependency>())
                    // We then specify what type we want to register these classes as.
                    // In this case, we want to register the types as all of its implemented interfaces.
                    // So if a type implements 3 interfaces; A, B, C, we'd end up with three separate registrations.
                    .AsImplementedInterfaces()
                    // And lastly, we specify the lifetime of these registrations.
                    .WithTransientLifetime()
                  );

            string serviceName = Configuration.GetSection("FILE:SERVICE").Value;

            if (serviceName == LinFile.LocalFileService)
            {
                services.AddTransient<IFileService, LocalFileService>();
            }
            else
            {
                services.AddTransient<IFileService, QiniuService>();
            }
            #endregion

            #region Swagger
            //Swagger重写PascalCase，改成SnakeCase模式
            services.TryAddEnumerable(ServiceDescriptor
                .Transient<IApiDescriptionProvider, SnakeCaseQueryParametersApiDescriptionProvider>());

            //Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo() { Title = "LinCms", Version = "v1" });
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

                string xmlPath = Path.Combine(AppContext.BaseDirectory, $"{typeof(Startup).Assembly.GetName().Name}.xml");
                options.IncludeXmlComments(xmlPath);

            });
            #endregion

            //将Handler注册到DI系统中
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 1024 * 1024 * 2;
                options.MultipartHeadersCountLimit = 10;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseHttpMethodOverride(new HttpMethodOverrideOptions { FormFieldName = "X-Http-Method-Override" });
            //env.EnvironmentName = EnvironmentName.Production;
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}
            app.UseHsts();
            app.UseStaticFiles();

            //app.UseMiddleware(typeof(CustomExceptionMiddleWare));

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            //// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            //// specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "LinCms");

                //c.RoutePrefix = string.Empty;
                //c.OAuthClientId("demo_api_swagger");//客服端名称
                //c.OAuthAppName("Demo API - Swagger-演示"); // 描述
            });

            app.UseCors(builder =>
            {
                string[] withOrigins = Configuration.GetSection("WithOrigins").Get<string[]>();

                builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins(withOrigins);
            });

            app.UseAuthentication();

            app.UseIdentityServer();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}
