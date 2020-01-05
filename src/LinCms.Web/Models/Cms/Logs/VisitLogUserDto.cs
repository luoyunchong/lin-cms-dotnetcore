﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinCms.Web.Models.Cms.Logs
{
    public class VisitLogUserDto
    {
        /// <summary>
        /// 总访问量
        /// </summary>
        public long TotalVisitsCount { get; set; }
        /// <summary>
        /// 总用户数
        /// </summary>
        public long TotalUserCount { get; set; }
        /// <summary>
        /// 新增访问量 (月)
        /// </summary>
        public long MonthVisitsCount { get; set; }

        /// <summary>
        /// 新增用户数(月)
        /// </summary>
        public long MonthUserCount { get; set; }
    }
}
