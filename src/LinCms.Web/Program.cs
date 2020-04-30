using System;
using System.IO;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Compact;

namespace LinCms.Web
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        public static async Task<int> Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration)
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.Debug(new RenderedCompactJsonFormatter())
                .WriteTo.File("Logs/log.txt",rollingInterval: RollingInterval.Day,rollOnFileSizeLimit: true)
                .CreateLogger();
            // Configuration.GetSection("exceptionless").Bind(Exceptionless.ExceptionlessClient.Default.Configuration);
            try
            {
                Log.Debug("init main");
                IHost webHost=CreateWebHostBuilder(args).Build();
                
                using (var scope = webHost.Services.CreateScope())
                {
                    // get the IpPolicyStore instance
                    var ipPolicyStore = scope.ServiceProvider.GetRequiredService<IIpPolicyStore>();

                    // seed IP data from appsettings
                    await ipPolicyStore.SeedAsync();
                }
                
               await  webHost.RunAsync();
               return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
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
                    webBuilder.UseStartup<Startup>().ConfigureAppConfiguration((host, config) =>
                    {
                        config.AddJsonFile($"RateLimitConfig.json", optional: true, reloadOnChange: true);
                    }); 
                })
                //.ConfigureLogging(logging =>
                //{
                //    logging.AddSerilog();
                //    logging.ClearProviders();
                //    logging.SetMinimumLevel(LogLevel.Trace);
                //})
                .UseSerilog();
    }
}
