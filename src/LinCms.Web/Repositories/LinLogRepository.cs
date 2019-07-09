using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FreeSql;
using LinCms.Web.Domain;

namespace LinCms.Web.Repositories
{
    /// <summary>
    /// 
    /// </summary>
    public class LinLogRepository : BaseRepository<LinLog>
    {
        public LinLogRepository(IFreeSql fsql, Expression<Func<LinLog, bool>> filter = null, Func<string, string> asTable = null) : base(fsql, filter, asTable)
        {
        }

        public List<LinLog> GetLogUsers()
        {
            List<LinLog> linLogs = Select.Page(1, 10).ToList();

            return linLogs;
        }
    }
}
