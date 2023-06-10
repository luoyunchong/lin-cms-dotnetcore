using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FreeSql;
using IGeekFan.FreeKit.Extras.Dto;
using LinCms.Entities;
using LinCms.Extensions;

namespace LinCms.Cms.Logs;

public class SerilogService : ISerilogService
{
    private readonly IBaseRepository<SerilogDO> _serilogBaseRepository;

    public SerilogService(IBaseRepository<SerilogDO> serilogBaseRepository)
    {
        _serilogBaseRepository = serilogBaseRepository;
    }

    public async Task<PagedResultDto<SerilogDO>> GetListAsync(SerilogSearchDto searchDto)
    {
        List<SerilogDO> serilogLists = await _serilogBaseRepository.Select
            .WhereIf(searchDto.Start.HasValue, r => r.Timestamp >= searchDto.Start.Value)
            .WhereIf(searchDto.End.HasValue, r => r.Timestamp <= searchDto.End.Value)
            .WhereIf(!string.IsNullOrEmpty(searchDto.Keyword), r => r.Message.Contains(searchDto.Keyword))
            .WhereIf(searchDto.LogLevel != null, r => r.Level == searchDto.LogLevel)
            .OrderByDescending(r => r.Id)
            .ToPagerListAsync(searchDto, out long count);
        return new PagedResultDto<SerilogDO>(serilogLists, count);
    }

    public async Task<LogDashboard> GetLogDashboard()
    {
        DateTime now = DateTime.Now;

        return new LogDashboard()
        {
            AllCount = await _serilogBaseRepository.Select.CountAsync(),
            TodayCount = await _serilogBaseRepository.Select.Where(x => x.Timestamp >= now.Date && x.Timestamp <= now.Date.AddHours(23).AddMinutes(59).AddSeconds(59)).CountAsync(),
            HourCount = await _serilogBaseRepository.Select.Where(x => x.Timestamp >= now.AddHours(-1) && x.Timestamp <= now).CountAsync(),
            UniqueCount = await _serilogBaseRepository.Select.WithSql(@"select count(*) AS TOTAL from app_serilog group by message").CountAsync(),
        };
    }
}