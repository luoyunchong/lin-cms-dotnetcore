using LinCms.Scaffolding.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace LinCms.Scaffolding
{
    class Program
    {

        static void ConfigureServices(ServiceCollection services)
        {
            // 配置日志
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            // 创建 config
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            services.Configure<SettingOptions>(configuration.GetSection(SettingOptions.Name));
            // 添加 services:
            //services.AddTransient<IEmailSender, CodeScaffolding>();
            // 添加 app
            services.AddTransient<App>();

        }

        static async Task Main(string[] args)
        {
            // 创建ServiceCollection
            var services = new ServiceCollection();

            ConfigureServices(services);

            // 创建ServiceProvider
            ServiceProvider serviceProvider = services.BuildServiceProvider();

            // app程序运行入口
            await serviceProvider.GetService<App>().Run(args);

        }
    }
}
