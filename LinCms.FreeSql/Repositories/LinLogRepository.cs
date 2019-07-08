using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using FreeSql;
using LinCms.Domain;

namespace LinCms.FreeSql.Repositories
{
    public class LinLogRepository : BaseRepository<LinLog, int>
    {
        public LinLogRepository(IFreeSql fsql, Expression<Func<LinLog, bool>> filter, Func<string, string> asTable = null) : base(fsql, filter, asTable)
        {
        }

        public List<LinLog> GetLogUsers()
        {
            List<LinLog> linLogs = Select.Page(1, 20).ToList();

            return linLogs;
        }
    }
}
