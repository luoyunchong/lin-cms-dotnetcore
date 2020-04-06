using LinCms.Core.Entities;

namespace LinCms.Core.IRepositories
{
    public interface ILogRepository : IAuditBaseRepository<LinLog>
    {
        void Create(LinLog linlog);
    }
}
