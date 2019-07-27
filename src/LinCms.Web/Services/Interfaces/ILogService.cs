using LinCms.Zero.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Web.Models.Logs;
using LinCms.Zero.Data;
using LinCms.Zero.Dependency;

namespace LinCms.Web.Services.Interfaces
{
    public interface ILogService
    {
        void InsertLog(LinLog linlog);
        PagedResultDto<LinLog> GetUserLogs(LogSearchDto searchDto);
        List<string> GetLoggedUsers(PageDto searchDto);
    }
}
