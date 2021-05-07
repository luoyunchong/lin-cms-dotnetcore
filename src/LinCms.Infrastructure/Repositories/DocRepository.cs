using FreeSql;
using LinCms.Entities.Base;
using LinCms.IRepositories;
using LinCms.Security;
namespace LinCms.Repositories
{
    public class DocRepository : AuditBaseRepository<Doc, long>, IDocRepository
    {
        public DocRepository(UnitOfWorkManager unitOfWork, ICurrentUser currentUser) : base(unitOfWork, currentUser)
        {
        }
    }
}
