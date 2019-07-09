using FreeSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using LinCms.Web.Domain;

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
                .UseAutoSyncStructure(true) //自动迁移实体的结构到数据库
                .UseMonitorCommand(cmd => Trace.WriteLine(cmd.CommandText))
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
            services.AddSingleton<IFreeSql>(Fsql);

            services.AddFreeRepository(filter =>
            {
                filter.Apply<ISoftDeleteAduitEntity>("SoftDelete", a => a.IsDeleted == false);
            }, this.GetType().Assembly);


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info() { Title = "LinCms", Version = "v1" });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseHttpMethodOverride(new HttpMethodOverrideOptions { FormFieldName = "X-Http-Method-Override" });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
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
                c.RoutePrefix = string.Empty;
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
