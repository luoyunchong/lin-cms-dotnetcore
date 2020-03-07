using FreeSql;
using LinCms.Core.Dependency;
using LinCms.Core.Entities;

namespace LinCms.Core.Repositories
{
    public interface ILogRepository : IScopedDependency
    {
        void Create(LinLog linlog);
    }
}
