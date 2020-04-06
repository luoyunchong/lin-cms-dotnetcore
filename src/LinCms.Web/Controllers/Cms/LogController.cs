using System.Collections.Generic;
using FreeSql;
using LinCms.Application.Cms.Logs;
using LinCms.Core.Aop;
using LinCms.Core.Data;
using LinCms.Core.Entities;
using LinCms.Application.Contracts.Cms.Logs;
using LinCms.Application.Contracts.Cms.Logs.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.Cms
{
    [Route("cms/log")]
    [ApiController]
    [DisableAuditing]
    public class LogController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogController(ILogService logService, BaseRepository<LinLog> linLogBaseRepository)
        {
            _logService = logService;
        }

        /// <summary>
        /// 查询日志记录的用户
        /// </summary>
        /// <returns></returns>
        [HttpGet("users")]
        [LinCmsAuthorize("查询日志记录的用户", "日志")]
        public List<string> GetUsers([FromQuery]PageDto pageDto)
        {
            return _logService.GetLoggedUsers(pageDto);
        }

        /// <summary>
        /// 日志浏览（人员，时间），分页展示
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [LinCmsAuthorize("查询所有日志", "日志")]
        public PagedResultDto<LinLog> GetLogs([FromQuery]LogSearchDto searchDto)
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
        public PagedResultDto<LinLog> GetUserLogs([FromQuery]LogSearchDto searchDto)
        {
            return _logService.GetUserLogs(searchDto);
        }

        [HttpGet("visitis")]
        public VisitLogUserDto GetUserAndVisits()
        {
            return _logService.GetUserAndVisits();

        }
    }
}
