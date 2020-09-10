using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Aop.Attributes;
using LinCms.Aop.Filter;
using LinCms.Cms.Logs;
using LinCms.Data;
using LinCms.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Controllers.Cms
{
    [ApiExplorerSettings(GroupName = "cms")]
    [Route("cms/log")]
    [ApiController]
    [DisableAuditing]
    public class LogController : ControllerBase
    {
        private readonly ILogService _logService;
        private readonly ISerilogService _serilogService;

        public LogController(ILogService logService, ISerilogService serilogService)
        {
            _logService = logService;
            _serilogService = serilogService;
        }

        /// <summary>
        /// 查询日志记录的用户
        /// </summary>
        /// <returns></returns>
        [HttpGet("users")]
        [LinCmsAuthorize("查询日志记录的用户", "日志")]
        public List<string> GetUsers([FromQuery] PageDto pageDto)
        {
            return _logService.GetLoggedUsers(pageDto);
        }

        /// <summary>
        /// 日志浏览（人员，时间），分页展示
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [LinCmsAuthorize("查询所有日志", "日志")]
        public PagedResultDto<LinLog> GetLogs([FromQuery] LogSearchDto searchDto)
        {
            return _logService.GetUserLogs(searchDto);
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
            return _logService.GetUserLogs(searchDto);
        }

        /// <summary>
        /// Serilog日志
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        [HttpGet("serilog")]
        [LinCmsAuthorize("Serilog日志", "日志")]
        public async Task<PagedResultDto<SerilogDO>> GetSerilogListAsync([FromQuery] SerilogSearchDto searchDto)
        {
            return await _serilogService.GetListAsync(searchDto);
        }

        [HttpGet("visitis")]
        public VisitLogUserDto GetUserAndVisits()
        {
            return _logService.GetUserAndVisits();
        }

        [HttpGet("dashboard")]
        public async Task<LogDashboard> GetLogDashboard()
        {
            return await _serilogService.GetLogDashboard();
        }
    }
}
