using FreeSql;
using IGeekFan.FreeKit.Extras.FreeSql;
using IGeekFan.FreeKit.Extras.Security;
using LinCms.Entities.Base;
using LinCms.IRepositories;
namespace LinCms.Repositories;

public class DocRepository : AuditDefaultRepository<Doc, long, long>, IDocRepository
{
    public DocRepository(UnitOfWorkManager unitOfWork, ICurrentUser currentUser) : base(unitOfWork, currentUser)
    {
    }
}