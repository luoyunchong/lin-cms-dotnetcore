using System;
using System.Linq.Expressions;
using FreeSql;
using LinCms.Core.Entities;
using LinCms.Core.IRepositories;
using LinCms.Core.Security;

namespace LinCms.Infrastructure.Repositories
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
