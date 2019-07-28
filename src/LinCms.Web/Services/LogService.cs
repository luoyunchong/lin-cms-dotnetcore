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
using LinCms.Zero.Security;

namespace LinCms.Web.Services
{
    public class LogService : ILogService
    {
        private readonly BaseRepository<LinLog> _linLogRepository;
        private readonly ICurrentUser _currentUser;
        public LogService(BaseRepository<LinLog> linLogRepository, ICurrentUser currentUser)
        {
            _linLogRepository = linLogRepository;
            _currentUser = currentUser;
        }

        public void InsertLog(LinLog linlog)
        {
            linlog.Time = DateTime.Now;
            linlog.UserName = _currentUser.UserName;
            linlog.UserId = _currentUser.Id ?? 0;

            _linLogRepository.Insert(linlog);
        }

        public PagedResultDto<LinLog> GetUserLogs(LogSearchDto searchDto)
        {
            List<LinLog> linLogs = _linLogRepository.Select
                .WhereIf(!string.IsNullOrEmpty(searchDto.Keyword), r => r.Message.Contains(searchDto.Keyword))
                .WhereIf(!string.IsNullOrEmpty(searchDto.Name), r => r.UserName.Contains(searchDto.Name))
                .WhereIf(searchDto.Start.HasValue, r => r.Time >= searchDto.Start.Value)
                .WhereIf(searchDto.End.HasValue, r => r.Time <= searchDto.End.Value)
                .OrderByDescending(r=>r.Id)
                .ToPagerList(searchDto, out long totalCount);

            return new PagedResultDto<LinLog>(linLogs,totalCount);

        }

        public List<string> GetLoggedUsers(PageDto searchDto)
        {
            List<string> linLogs = _linLogRepository.Select.Where(r => !string.IsNullOrEmpty(r.UserName)).OrderByDescending(r => r.Id)
                .ToPagerList(searchDto, out _).Select(r => r.UserName).ToList();

            return linLogs;
        }
    }
}
