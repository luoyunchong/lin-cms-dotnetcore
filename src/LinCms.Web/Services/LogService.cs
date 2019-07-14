using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSql;
using LinCms.Web.Models.Logs;
using LinCms.Web.Services.Interfaces;
using LinCms.Zero.Data;
using LinCms.Zero.Domain;
using LinCms.Zero.Extensions;

namespace LinCms.Web.Services
{
    public class LogService : ILogService
    {
        private readonly BaseRepository<LinLog> _linLogRepository;
        public LogService(BaseRepository<LinLog> linLogRepository)
        {
            _linLogRepository = linLogRepository;
        }
        public void InsertLog(LinLog linlog)
        {
            linlog.Time = DateTime.Now;
            _linLogRepository.Insert(linlog);
        }

        public PagedResultDto<LinLog> GetLogUsers(LogSearchDto searchDto)
        {
            List<LinLog> linLogs = _linLogRepository.Select
                .WhereIf(!string.IsNullOrEmpty(searchDto.Keyword), r => r.Message.Contains(searchDto.Keyword))
                .WhereIf(!string.IsNullOrEmpty(searchDto.Name), r => r.UserName.Contains(searchDto.Name))
                .WhereIf(searchDto.Start.HasValue, r => r.Time >= searchDto.Start.Value)
                .WhereIf(searchDto.End.HasValue, r => r.Time <= searchDto.End.Value)
                .ToPagerList(searchDto, out long totalCount);

            return new PagedResultDto<LinLog>(linLogs,totalCount);

        }

        public List<string> GetLoggedUsers(PageDto searchDto)
        {
            List<string> linLogs = _linLogRepository.Select
                .ToPagerList(searchDto, out long totalCount).Select(r => r.UserName).ToList();

            return linLogs;
        }
    }
}
