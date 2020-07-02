using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using AutoMapper;
using DotNetCore.CAP.Messages;
using DotNetCore.Security;
using HealthChecks.UI.Client;
using LinCms.Application.Cms.Users;
using LinCms.Application.Contracts;
using LinCms.Core.Aop.Filter;
using LinCms.Core.Aop.Middleware;
using LinCms.Core.Common;
using LinCms.Core.Data;
using LinCms.Core.Data.Enums;
using LinCms.Core.Entities;
using LinCms.Core.Extensions;
using LinCms.Plugins.Poem.Services;
using LinCms.Web.Middleware;
using LinCms.Web.SnakeCaseQuery;
using LinCms.Web.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Savorboard.CAP.InMemoryMessageQueue;
using Serilog;

namespace LinCms.Web.Startup
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddContext(Configuration);
            services.AddCsRedisCore(Configuration);
            services.AddSecurity(Configuration);

            #region AddJwtBearer
            var jsonWebTokenSettings = services.BuildServiceProvider().GetRequiredService<JsonWebTokenSettings>();
            services.AddAuthentication(opts =>
                {
                    opts.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.LoginPath = "/cms/oauth2/signin";
                    options.LogoutPath = "/cms/oauth2/signout";
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    //identityserver4 地址 也就是本项目地址
                    options.Authority = Configuration["Service:Authority"];
                    options.RequireHttpsMetadata = false;
                    options.Audience = Configuration["Service:Name"];

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // The signing key must match!
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jsonWebTokenSettings.Key)),

                        // Validate the JWT Issuer (iss) claim
                        ValidateIssuer = true,
                        ValidIssuer = jsonWebTokenSettings.Issuer,

                        // Validate the JWT Audience (aud) claim
                        ValidateAudience = true,
                        ValidAudience = jsonWebTokenSettings.Audience,

                        // Validate the token expiry
                        ValidateLifetime = true,

                        // If you want to allow a certain amount of clock drift, set thatValidIssuer  here
                        //ClockSkew = TimeSpan.Zero
                    };

                    //options.TokenValidationParameters = new TokenValidationParameters()
                    //{
                    //    ClockSkew = TimeSpan.Zero   //偏移设置为了0s,用于测试过期策略,完全按照access_token的过期时间策略，默认原本为5分钟
                    //};


                    //使用Authorize设置为需要登录时，返回json格式数据。
                    options.Events = new JwtBearerEvents()
                    {
                        OnAuthenticationFailed = context =>
                        {
                            //Token expired
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }

                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            //此处代码为终止.Net Core默认的返回类型和数据结果，这个很重要哦
                            context.HandleResponse();

                            string message;
                            ErrorCode errorCode;
                            int statusCode = StatusCodes.Status401Unauthorized;

                            if (context.Error == "invalid_token" &&
                                context.ErrorDescription == "The token is expired")
                            {
                                message = "令牌过期";
                                errorCode = ErrorCode.TokenExpired;
                                statusCode = StatusCodes.Status422UnprocessableEntity;
                            }
                            else if (context.Error == "invalid_token" && context.ErrorDescription.IsNullOrEmpty())
                            {
                                message = "令牌失效";
                                errorCode = ErrorCode.TokenInvalidation;
                            }

                            else
                            {
                                message = "请先登录" + context.ErrorDescription; //""认证失败，请检查请求头或者重新登录";
                                errorCode = ErrorCode.AuthenticationFailed;
                            }

                            context.Response.ContentType = "application/json";
                            context.Response.StatusCode = statusCode;
                            context.Response.WriteAsync(new UnifyResponseDto(errorCode, message, context.HttpContext).ToString());

                            return Task.FromResult(0);
                        }
                    };
                })
                .AddGitHub(options =>
                {
                    options.ClientId = Configuration["Authentication:GitHub:ClientId"];
                    options.ClientSecret = Configuration["Authentication:GitHub:ClientSecret"];
                    options.Scope.Add("user:email");
                    options.ClaimActions.MapJsonKey(LinConsts.Claims.AvatarUrl, "avatar_url");
                    //登录成功后可通过  authenticateResult.Principal.FindFirst(ClaimTypes.Uri)?.Value;  得到GitHub头像
                    options.ClaimActions.MapJsonKey(LinConsts.Claims.BIO, "bio");
                    options.ClaimActions.MapJsonKey(LinConsts.Claims.BlogAddress, "blog");
                })
                .AddQQ(options =>
                {
                    options.ClientId = Configuration["Authentication:QQ:ClientId"];
                    options.ClientSecret = Configuration["Authentication:QQ:ClientSecret"];
                });

            #endregion


            services.AddAutoMapper(typeof(UserProfile).Assembly, typeof(PoemProfile).Assembly);

            services.AddCors();

            #region Mvc

            services.AddControllers(options =>
                {
                    options.ValueProviderFactories.Add(new ValueProviderFactory()); //设置SnakeCase形式的QueryString参数
                    options.Filters.Add<LogActionFilterAttribute>(); // 添加请求方法时的日志记录过滤器
                    // options.Filters.Add<LinCmsExceptionFilter>(); // 添加请求方法时的日志记录过滤器
                })
                .AddNewtonsoftJson(opt =>
                {
                    opt.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:MM:ss";
                    // 设置自定义时间戳格式
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
                        },
                    };
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.SuppressConsumesConstraintForFormFileParameters =
                        true; //SuppressUseValidationProblemDetailsForInvalidModelStateResponses;
                    //自定义 BadRequest 响应
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var problemDetails = new ValidationProblemDetails(context.ModelState);

                        var resultDto = new UnifyResponseDto(ErrorCode.ParameterError, problemDetails.Errors,
                            context.HttpContext);

                        return new BadRequestObjectResult(resultDto)
                        {
                            ContentTypes = { "application/json" }
                        };
                    };
                });

            #endregion

            services.AddDIServices();

            #region Swagger

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
                        Email = "luoyunchong@foxmail.com",
                        Url = new Uri("https://www.cnblogs.com/igeekfan/")
                    },
                    License = new OpenApiLicense
                    {
                        Name = ApiName + " 官方文档",
                        Url = new Uri(
                            "https://luoyunchong.github.io/vovo-docs/dotnetcore/lin-cms/dotnetcore-start.html")
                    }
                });

                var security = new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference()
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                };
                options.AddSecurityRequirement(security); //添加一个必须的全局安全信息，和AddSecurityDefinition方法指定的方案名称要一致，这里是Bearer。
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 参数结构: \"Authorization: Bearer {token}\"",
                    Name = "Authorization", //jwt默认的参数名称
                    In = ParameterLocation.Header, //jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey
                });
                try
                {
                    string xmlPath = Path.Combine(AppContext.BaseDirectory, $"{typeof(Startup).Assembly.GetName().Name}.xml");
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
            });

            #endregion


            //应用程序级别设置

            services.Configure<FormOptions>(options =>
            {
                //单个文件上传的大小限制为8 MB      默认134217728 应该是128MB
                options.MultipartBodyLengthLimit = 1024 * 1024 * 8; //8MB
            });

            #region 分布式事务一致性CAP

            IConfigurationSection configurationSection = Configuration.GetSection("ConnectionStrings:MySql");
            services.AddCap(x =>
            {
                x.UseMySql(configurationSection.Value);

                bool isEnableInMemoryQueue = Configuration["CAP:InMemoryQueue:IsEnabled"].ToBoolean();
                if (isEnableInMemoryQueue)
                {
                    x.UseInMemoryMessageQueue();
                }

                bool isEnableRabbitMq = Configuration["CAP:RabbitMQ:IsEnabled"].ToBoolean();
                if (isEnableRabbitMq)
                {
                    x.UseRabbitMQ(options =>
                    {
                        options.HostName = Configuration["CAP:RabbitMQ:HostName"];
                        options.UserName = Configuration["CAP:RabbitMQ:UserName"];
                        options.Password = Configuration["CAP:RabbitMQ:Password"];
                        options.VirtualHost = Configuration["CAP:RabbitMQ:VirtualHost"];
                    });
                }

                x.UseDashboard();
                x.FailedRetryCount = 5;
                x.FailedThresholdCallback = (type) =>
                {
                    Console.WriteLine(
                        $@"A message of type {type} failed after executing {x.FailedRetryCount} several times, requiring manual troubleshooting. Message name: {type.Message.GetName()}");
                };
            });

            #endregion

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
            //之前请注入AddCsRedisCore，内部实现IDistributedCache接口
            services.AddIpRateLimiting(Configuration);

            services.AddHealthChecks();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new AutofacModule());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseForwardedHeaders();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseSerilogRequestLogging(opts =>
            {
                opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest;
                opts.GetLevel = LogHelper.ExcludeHealthChecks;// Use the custom level
            });

            //异常中间件应放在MVC执行事务的中件间的前面，否则异常时UnitOfWorkMiddleware无法catch异常
            app.UseMiddleware(typeof(CustomExceptionMiddleWare));

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "LinCms");
                //c.RoutePrefix = string.Empty;
                //c.OAuthClientId(Configuration["Service:ClientId"]);//客服端名称
                //c.OAuthAppName(Configuration["Service:Name"]); // 描述
            });

            app.UseCors(builder =>
            {
                string[] withOrigins = Configuration.GetSection("WithOrigins").Get<string[]>();
                builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins(withOrigins);
            });

            //认证中间件
            app.UseAuthentication();

            app.UseMiddleware<IpLimitMiddleware>();

            app.UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapHealthChecks("/health", new HealthCheckOptions
                    {
                        Predicate = _ => true,
                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                    });
                });
        }
    }
}