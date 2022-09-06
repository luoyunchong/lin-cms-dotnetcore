using FreeSql;
using IGeekFan.FreeKit.Extras.FreeSql;
using IGeekFan.FreeKit.Extras.Security;
using LinCms.Entities;
using LinCms.IRepositories;

namespace LinCms.Repositories;

/// <summary>
/// 当需要给仓储增加方法时，在此方法中增加，并在构造函数中注入BookRepository
/// </summary>
public class BookRepository : AuditDefaultRepository<Book, long, long>, IBookRepository
{
    public BookRepository(UnitOfWorkManager unitOfWork, ICurrentUser currentUser) : base(unitOfWork, currentUser)
    {
    }

}