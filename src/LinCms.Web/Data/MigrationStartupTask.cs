using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LinCms.Data
{
    public class MigrationStartupTask
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MigrationStartupTask> _logger;
        public MigrationStartupTask(IServiceProvider serviceProvider, ILogger<MigrationStartupTask> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                IDataSeedContributor dataSeedContributor = scope.ServiceProvider.GetRequiredService<IDataSeedContributor>();
                await dataSeedContributor.SeedPermissionAsync();
                await dataSeedContributor.InitAdminPermission();
            }
            catch (Exception ex)
            {
                _logger.LogError("初始化数据失败！！！",ex);
            };
        }

    }
}
