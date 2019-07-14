using AutoMapper;
using FreeSql;
using FreeSql.Internal;
using LinCms.Web.Aop;
using LinCms.Web.Data;
using LinCms.Web.Services;
using LinCms.Web.Services.Interfaces;
using LinCms.Zero.Domain;
using LinCms.Zero.Exceptions;
using LinCms.Zero.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;
using System.Diagnostics;

namespace LinCms.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            IConfigurationSection configurationSection = Configuration.GetSection("ConnectionStrings:Default");

            Fsql = new FreeSqlBuilder()
                .UseConnectionString(DataType.MySql, configurationSection.Value)
                .UseEntityPropertyNameConvert(StringConvertType.PascalCaseToUnderscoreWithLower)//全局转换实体属性名方法 https://github.com/2881099/FreeSql/pull/60
                .UseAutoSyncStructure(true) //自动迁移实体的结构到数据库
                .UseMonitorCommand(cmd => Trace.WriteLine(cmd.CommandText))
                .UseSyncStructureToLower(true) // 转小写同步结构
                .Build();

            Fsql.Aop.CurdAfter = (s, e) =>
            {
                if (e.ElapsedMilliseconds > 200)
                {
                    //记录日志
                    //发送短信给负责人
                }
            };
        }

        public IConfiguration Configuration { get; }

        public static IFreeSql Fsql { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            InMemoryConfiguration.Configuration = this.Configuration;
            services.AddSingleton(Fsql);

            services.AddFreeRepository(filter =>
            {
                filter.Apply<ISoftDeleteAduitEntity>("SoftDelete", a => a.IsDeleted == false);
            }, GetType().Assembly);


            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(InMemoryConfiguration.GetIdentityResources())
                .AddInMemoryApiResources(InMemoryConfiguration.GetApis())
                .AddInMemoryClients(InMemoryConfiguration.GetClients())
                .AddProfileService<CustomProfileService>()
                .AddResourceOwnerValidator<CustomResourceOwnerPasswordValidator>();


            #region AddAuthentication\AddIdentityServerAuthentication 
            //AddAuthentication()是把验证服务注册到DI, 并配置了Bearer作为默认模式.

            //AddIdentityServerAuthentication()是在DI注册了token验证的处理者.
            //由于是本地运行, 所以就不使用https了, RequireHttpsMetadata = false.如果是生产环境, 一定要使用https.
            //Authority指定Authorization Server的地址.
            //ApiName要和Authorization Server里面配置ApiResource的name一样.
            #endregion

            //services
            //    .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            //    .AddIdentityServerAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
            //    {
            //        options.RequireHttpsMetadata = false; // for dev env
            //        options.Authority = $"{Configuration["Identity:Protocol"]}://{Configuration["Identity:IP"]}:{Configuration["Identity:Port"]}"; ;
            //        options.ApiName = Configuration["Service:Name"]; // match with configuration in IdentityServer
            //    });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    //identityserver4 地址 也就是本项目地址
                    options.Authority = $"{Configuration["Identity:Protocol"]}://{Configuration["Identity:IP"]}:{Configuration["Identity:Port"]}"; ;
                    options.RequireHttpsMetadata = false;
                    options.Audience = Configuration["Service:Name"];
                });

            services.AddAutoMapper(GetType().Assembly);

            services.AddMvc(options=> {
                options.Filters.Add<LogActionAttribute>(); // 添加日志记录过滤器

            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                    .AddJsonOptions(opt =>
                    {
                        //opt.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:MM:ss";
                        //设置时间戳格式
                        opt.SerializerSettings.Converters = new List<JsonConverter>()
                        {
                            new LinCmsTimeConverter()
                        };
                        // 设置下划线
                        opt.SerializerSettings.ContractResolver = new DefaultContractResolver()
                        {
                            NamingStrategy = new SnakeCaseNamingStrategy()
                        };
                    });

            services.AddScoped<ILogService, LogService>();
      
            //Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info() { Title = "LinCms", Version = "v1" });
                var security = new Dictionary<string, IEnumerable<string>> { { "Bearer", new string[] { } }, };
                options.AddSecurityRequirement(security);//添加一个必须的全局安全信息，和AddSecurityDefinition方法指定的方案名称要一致，这里是Bearer。
                options.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 参数结构: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",//jwt默认的参数名称
                    In = "header",//jwt默认存放Authorization信息的位置(请求头中)
                    Type = "apiKey"
                });
                options.OperationFilter<SwaggerFileHeaderParameter>();
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseHttpMethodOverride(new HttpMethodOverrideOptions { FormFieldName = "X-Http-Method-Override" });
            //env.EnvironmentName = EnvironmentName.Production;
            if (env.IsDevelopment())
            {

                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware<CustomExceptionMiddleWare>();
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "LinCms");

                //c.RoutePrefix = string.Empty;
                //c.OAuthClientId("demo_api_swagger");//客服端名称
                //c.OAuthAppName("Demo API - Swagger-演示"); // 描述
            });



            app.UseAuthentication();

            app.UseIdentityServer();

            app.UseHttpsRedirection();

            app.UseMvc();
        }

    }
}
