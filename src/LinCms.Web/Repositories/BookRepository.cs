using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FreeSql;
using LinCms.Zero.Domain;
using LinCms.Zero.Security;

namespace LinCms.Web.Repositories
{

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
