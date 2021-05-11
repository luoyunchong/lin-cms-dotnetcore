using System.IO;
using System.Threading.Tasks;
using LinCms.Scaffolding.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace LinCms.Scaffolding
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = AppStartup();

            var service = ActivatorUtilities.CreateInstance<App>(host.Services);

            await service.RunAsync(args);

        }

        static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                ;
        }

        static IHost AppStartup()
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            BuildConfig(builder);
            IConfigurationRoot configuration = builder.Build();
       
            Log.Logger = new LoggerConfiguration() 
                            .ReadFrom.Configuration(configuration)
                            .Enrich.FromLogContext() 
                            .CreateLogger();

            Log.Logger.Information("Application Starting");

            var host = Host.CreateDefaultBuilder() 
                        .ConfigureServices((context, services) =>
                        { 
                            services.Configure<SettingOptions>(configuration.GetSection(SettingOptions.Name));
                            services.AddTransient<App>();
                        })
                        .ConfigureAppConfiguration((host, config) =>
                        {
                        })
                        .UseSerilog() 
                        .Build();

            return host;
        }
    }
}