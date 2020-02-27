using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NetEscapades.AspNetCore.StartupTasks;

namespace LinCms.Web.Data
{
    public class MigrationStartupTask : IStartupTask
    {
        private readonly IServiceProvider _serviceProvider;

        public MigrationStartupTask(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                IDataSeedContributor dataSeedContributor = scope.ServiceProvider.GetRequiredService<IDataSeedContributor>();

                await dataSeedContributor.SeedAsync();
            }
        }

        public Task ShutdownAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
