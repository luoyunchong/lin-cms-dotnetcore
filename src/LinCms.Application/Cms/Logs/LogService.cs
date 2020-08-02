using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Data;
using LinCms.Entities;
using LinCms.Extensions;
using LinCms.IRepositories;
using LinCms.Security;

namespace LinCms.Cms.Logs
{
    public class LogService :ApplicationService, ILogService
    {
        private readonly IAuditBaseRepository<LinLog> _linLogRepository;
        private readonly IUserRepository _linUserAuditBaseRepository;
        public LogService(IAuditBaseRepository<LinLog> linLogRepository,IUserRepository linUserAuditBaseRepository)
        {
            _linLogRepository = linLogRepository;
            _linUserAuditBaseRepository = linUserAuditBaseRepository;
        }

        public async Task CreateAsync(LinLog linlog)
        {
            linlog.CreateTime = DateTime.Now;
            linlog.Username = CurrentUser.UserName;
            linlog.UserId = CurrentUser.Id ?? 0;

            await _linLogRepository.InsertAsync(linlog);
        }

        public PagedResultDto<LinLog> GetUserLogs(LogSearchDto searchDto)
        {
            List<LinLog> linLogs = _linLogRepository.Select
                .WhereIf(!string.IsNullOrEmpty(searchDto.Keyword), r => r.Message.Contains(searchDto.Keyword))
                .WhereIf(!string.IsNullOrEmpty(searchDto.Name), r => r.Username.Contains(searchDto.Name))
                .WhereIf(searchDto.Start.HasValue, r => r.CreateTime >= searchDto.Start.Value)
                .WhereIf(searchDto.End.HasValue, r => r.CreateTime <= searchDto.End.Value)
                .OrderByDescending(r => r.Id)
                .ToPagerList(searchDto, out long totalCount);

            return new PagedResultDto<LinLog>(linLogs, totalCount);

        }

        public List<string> GetLoggedUsers(PageDto searchDto)
        {
            List<string> linLogs = _linLogRepository.Select
                .Where(r => !string.IsNullOrEmpty(r.Username))
                .Distinct()
                .ToList(r => r.Username);

            return linLogs;
        }


        public VisitLogUserDto GetUserAndVisits()
        {
            DateTime now = DateTime.Now;
            DateTime lastMonth = DateTime.Now.AddMonths(-1);

            long totalVisitsCount = _linLogRepository.Select.Count();
            long totalUserCount = _linUserAuditBaseRepository.Select.Count();
            long monthVisitsCount = _linLogRepository.Select.Where(r => r.CreateTime >= lastMonth && r.CreateTime <= now).Count();
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
