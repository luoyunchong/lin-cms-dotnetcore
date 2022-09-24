using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dependency;
using LinCms.Data;

namespace LinCms.FreeSql;

public interface IDataSeedContributor: ISingletonDependency
{

    Task InitAdminPermission();

    Task SeedPermissionAsync(List<PermissionDefinition> linCmsAttributes, CancellationToken cancellationToken);

}