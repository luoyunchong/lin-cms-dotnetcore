using System;
using System.Collections.Generic;
using System.Linq;
using FreeSql;
using LinCms.Web.Models.Cms.Logs;
using LinCms.Web.Services.Cms.Interfaces;
using LinCms.Zero.Data;
using LinCms.Zero.Domain;
using LinCms.Zero.Extensions;
using LinCms.Zero.Repositories;
using LinCms.Zero.Security;

namespace LinCms.Web.Services.Cms
{
    public class LogService : ILogService
    {
        private readonly BaseRepository<LinLog> _linLogRepository;
        private readonly ICurrentUser _currentUser;
        private readonly AuditBaseRepository<LinUser> _linUserAuditBaseRepository;
        public LogService(BaseRepository<LinLog> linLogRepository, ICurrentUser currentUser, AuditBaseRepository<LinUser> linUserAuditBaseRepository)
        {
            _linLogRepository = linLogRepository;
            _currentUser = currentUser;
            _linUserAuditBaseRepository = linUserAuditBaseRepository;
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
                .OrderByDescending(r => r.Id)
                .ToPagerList(searchDto, out long totalCount);

            return new PagedResultDto<LinLog>(linLogs, totalCount);

        }

        public List<string> GetLoggedUsers(PageDto searchDto)
        {
            List<string> linLogs = _linLogRepository.Select.Where(r => !string.IsNullOrEmpty(r.UserName)).OrderByDescending(r => r.Id)
                .Select(r => r.UserName).Distinct().ToList();
            //.ToPagerList(searchDto, out _)
            return linLogs;
        }


        public VisitLogUserDto GetUserAndVisits()
        {
            DateTime now = DateTime.Now;
            DateTime lastMonth = DateTime.Now.AddMonths(-1);

            long totalVisitsCount = _linLogRepository.Select.Count();
            long totalUserCount = _linUserAuditBaseRepository.Select.Count();
            long monthVisitsCount = _linLogRepository.Select.Where(r => r.Time >= lastMonth && r.Time <= now).Count();
            long monthUserCount = _linUserAuditBaseRepository.Select.Where(r => r.CreateTime >= lastMonth && r.CreateTime <= now).Count();

            return new VisitLogUserDto()
            {
                TotalVisitsCount = totalVisitsCount,
                TotalUserCount = totalUserCount,
                MonthVisitsCount = monthVisitsCount,
                MonthUserCount = monthUserCount
            };
        }
    }
}
