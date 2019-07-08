using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using FreeSql;
using LinCms.Domain;

namespace LinCms.FreeSql.Repositories
{
    public class LinUserRepository : BaseRepository<LinUser, int>
    {
        public LinUserRepository(IFreeSql fsql, Expression<Func<LinUser, bool>> filter, Func<string, string> asTable = null) : base(fsql, filter, asTable)
        {
        }


    }
}
