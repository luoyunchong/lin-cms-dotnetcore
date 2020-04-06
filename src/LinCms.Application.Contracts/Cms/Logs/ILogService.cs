using System.Collections.Generic;
using LinCms.Application.Contracts.Cms.Logs.Dtos;
using LinCms.Core.Data;
using LinCms.Core.Entities;

namespace LinCms.Application.Contracts.Cms.Logs
{
    public interface ILogService
    {
        void CreateLog(LinLog linlog);

        PagedResultDto<LinLog> GetUserLogs(LogSearchDto searchDto);

        List<string> GetLoggedUsers(PageDto searchDto);

        /// <summary>
        /// 管理端访问与用户统计
        /// </summary>
        /// <returns></returns>
        VisitLogUserDto GetUserAndVisits();
    }
}
