using FreeSql;
using LinCms.Entities;
using LinCms.IRepositories;
using LinCms.Security;

namespace LinCms.Repositories
{
    /// <summary>
    /// 当需要给仓储增加方法时，在此方法中增加，并在构造函数中注入BookRepository
    /// </summary>
    public class BookRepository : AuditBaseRepository<Book>, IBookRepository
    {
        private readonly ICurrentUser _currentUser;
        public BookRepository(UnitOfWorkManager unitOfWork, ICurrentUser currentUser): base(unitOfWork, currentUser)
        {
            _currentUser = currentUser;
        }

    }
}
