using FreeSql;
using LinCms.Web.Models.Logs;
using LinCms.Zero.Data;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LinCms.Zero.Domain;
using LinCms.Zero.Extensions;
using System.Linq;

namespace LinCms.Web.Repositories
{
    /// <summary>
    /// 当需要给仓储增加方法时，在此方法中增加，并在构造函数中注入LinLogRepository
    /// </summary>
    public class LinLogRepository : BaseRepository<LinLog>
    {

        public LinLogRepository(IFreeSql fsql, Expression<Func<LinLog, bool>> filter = null, Func<string, string> asTable = null) : base(fsql, filter, asTable)
        {
        }

    }
}
