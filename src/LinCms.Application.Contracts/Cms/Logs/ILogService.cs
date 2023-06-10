using System.Collections.Generic;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dto;
using LinCms.Data;
using LinCms.Entities;

namespace LinCms.Cms.Logs;

public interface ILogService
{
    Task CreateAsync(LinLog linlog);
    PagedResultDto<LinLog> GetUserLogs(LogSearchDto searchDto);

    List<string> GetLoggedUsers(PageDto searchDto);

    /// <summary>
    /// 管理端访问与用户统计
    /// </summary>
    /// <returns></returns>
    VisitLogUserDto GetUserAndVisits();
}