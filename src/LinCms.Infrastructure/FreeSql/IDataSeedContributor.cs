using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LinCms.Data;

namespace LinCms.FreeSql
{
    public interface IDataSeedContributor
    {

        Task InitAdminPermission();

        Task SeedPermissionAsync(List<PermissionDefinition> linCmsAttributes, CancellationToken cancellationToken);

    }
}
