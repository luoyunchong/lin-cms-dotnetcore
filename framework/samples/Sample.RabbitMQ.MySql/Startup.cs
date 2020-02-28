using System;
using DotNetCore.CAP.Messages;
using FreeSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Sample.RabbitMQ.MySql
{
    public class Startup
    {
        public IFreeSql Fsql { get; }
        public IConfiguration Configuration { get; }

        private string connectionString = @"Data Source=localhost;Port=3306;User ID=root;Password=123456;Initial Catalog=captest;Charset=utf8mb4;SslMode=none;Max pool size=10";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            Fsql = new FreeSqlBuilder()
                .UseConnectionString(DataType.MySql, connectionString)
                .UseAutoSyncStructure(true)
                .Build();
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>();
            services.AddSingleton<IFreeSql>(Fsql);
            services.AddCap(x =>
            {

                x.UseMySql(connectionString);

                x.UseRabbitMQ("localhost");
                x.UseDashboard();
                x.FailedRetryCount = 5;
                x.FailedThresholdCallback = (type, msg) =>
                {
                    Console.WriteLine(
                        $@"A message of type {type} failed after executing {x.FailedRetryCount} several times, requiring manual troubleshooting. Message name: {msg.GetName()}");
                };
            });

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
