using LinCms.Zero.Domain;
using System.Collections.Generic;
using LinCms.Web.Models.Cms.Logs;
using LinCms.Zero.Data;

namespace LinCms.Web.Services.Interfaces
{
    public interface ILogService
    {
        void InsertLog(LinLog linlog);
        PagedResultDto<LinLog> GetUserLogs(LogSearchDto searchDto);
        List<string> GetLoggedUsers(PageDto searchDto);
    }
}
