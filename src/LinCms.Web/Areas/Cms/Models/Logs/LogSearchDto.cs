using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Web.Data;

namespace LinCms.Web.Areas.Cms.Models.Logs
{
    public class LogSearchDto : PageDto
    {
        public string Keyword { get; set; }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 起始时间
        /// </summary>
        public DateTime? Start { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? End { get; set; }
    }
}
