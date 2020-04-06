using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace LinCms.Web.Data
{
    public class MigrationStartupTask
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

    }
}
