using System.Collections.Generic;
using LinCms.Web.Models.Cms.Logs;
using LinCms.Zero.Data;
using LinCms.Zero.Domain;

namespace LinCms.Web.Services.Cms.Interfaces
{
    public interface ILogService
    {
        void InsertLog(LinLog linlog);
        PagedResultDto<LinLog> GetUserLogs(LogSearchDto searchDto);
        List<string> GetLoggedUsers(PageDto searchDto);
    }
}
