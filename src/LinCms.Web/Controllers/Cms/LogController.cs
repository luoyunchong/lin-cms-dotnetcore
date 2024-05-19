using System.Collections.Generic;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dto;
using LinCms.Aop.Attributes;
using LinCms.Aop.Filter;
using LinCms.Cms.Logs;
using LinCms.Data;
using LinCms.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Controllers.Cms;

/// <summary>
/// 日志
/// </summary>
[ApiExplorerSettings(GroupName = "cms")]
[Route("cms/log")]
[ApiController]
[DisableAuditing]
public class LogController(ILogService logService, ISerilogService serilogService) : ControllerBase
{
    /// <summary>
    /// 查询日志记录的用户
    /// </summary>
    /// <returns></returns>
    [HttpGet("users")]
    [LinCmsAuthorize("查询日志记录的用户", "日志")]
    public List<string> GetUsers([FromQuery] PageDto pageDto)
    {
        return logService.GetLoggedUsers(pageDto);
    }

    /// <summary>
    /// 日志浏览（人员，时间），分页展示
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [LinCmsAuthorize("查询所有日志", "日志")]
    public PagedResultDto<LinLog> GetLogs([FromQuery] LogSearchDto searchDto)
    {
        return logService.GetUserLogs(searchDto);
    }

    /// <summary>
    /// 日志搜素（人员，时间）（内容）， 分页展示
    /// </summary>
    /// <param name="searchDto"></param>
    /// <returns></returns>
    [HttpGet("search")]
    [LinCmsAuthorize("搜索日志", "日志")]
    public PagedResultDto<LinLog> GetUserLogs([FromQuery] LogSearchDto searchDto)
    {
        return logService.GetUserLogs(searchDto);
    }

    /// <summary>
    /// Serilog日志
    /// </summary>
    /// <param name="searchDto"></param>
    /// <returns></returns>
    [HttpGet("serilog")]
    [LinCmsAuthorize("Serilog日志", "日志")]
    public Task<PagedResultDto<SerilogDO>> GetSerilogListAsync([FromQuery] SerilogSearchDto searchDto)
    {
        return serilogService.GetListAsync(searchDto);
    }

    [HttpGet("visitis")]
    public VisitLogUserDto GetUserAndVisits()
    {
        return logService.GetUserAndVisits();
    }

    [HttpGet("dashboard")]
    public Task<LogDashboard> GetLogDashboard()
    {
        return serilogService.GetLogDashboard();
    }
}