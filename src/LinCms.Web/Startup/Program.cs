using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace LinCms.Startup
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        public static async Task Main(string[] args)
        {
#if DEBUG
            Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
#endif

            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            try
            {
                Log.Information("init main");
                IHost webHost = CreateWebHostBuilder(args).Build();
                try
                {
                    using var scope = webHost.Services.CreateScope();
                    // get the IpPolicyStore instance
                    var ipPolicyStore = scope.ServiceProvider.GetRequiredService<IIpPolicyStore>();
                    // seed IP data from appsettings
                    await ipPolicyStore.SeedAsync();
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "IIpPolicyStore RUN Error");
                }
                await webHost.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IHostBuilder CreateWebHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel((context, options) =>
                    {
                        //设置应用服务器Kestrel请求体最大为50MB，默认为28.6MB
                        options.Limits.MaxRequestBodySize = 1024 * 1024 * 50;
                    });
                    webBuilder.UseStartup<Startup>().ConfigureAppConfiguration((host, config) =>
                    {
                        config.AddJsonFile($"RateLimitConfig.json", optional: true, reloadOnChange: true);
                    });
                })
                .UseSerilog();
    }
}
