using System.Collections.Generic;
using LinCms.Web.Data.Authorization;
using LinCms.Web.Models.Cms.Logs;
using LinCms.Web.Services.Interfaces;
using LinCms.Zero.Aop;
using LinCms.Zero.Data;
using LinCms.Zero.Domain;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.Cms
{
    [Route("cms/log")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogController(ILogService logService)
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

    }
}
