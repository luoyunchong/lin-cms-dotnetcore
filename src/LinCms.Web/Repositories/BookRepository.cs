using System;
using System.Linq.Expressions;
using LinCms.Zero;
using LinCms.Zero.Domain;
using LinCms.Zero.Repositories;
using LinCms.Zero.Security;

namespace LinCms.Web.Repositories
{
    /// <summary>
    /// 当需要给仓储增加方法时，在此方法中增加，并在构造函数中注入BookRepository
    /// </summary>
    public class BookRepository : AuditBaseRepository<Book>
    {
        private readonly ICurrentUser _currentUser;
        public BookRepository(ICurrentUser currentUser, IFreeSql fsql, Expression<Func<Book, bool>> filter = null, Func<string, string> asTable = null) 
            : base(currentUser,fsql, filter, asTable)
        {
            _currentUser = currentUser;
        }

    }
}
