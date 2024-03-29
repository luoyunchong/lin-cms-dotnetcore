﻿using FreeSql.DataAnnotations;
using IGeekFan.FreeKit.Extras.AuditEntity;

namespace LinCms.Entities.Base
{
    /// <summary>
    /// 文档---示例代码生成。
    /// </summary>
    public class Doc : FullAuditEntity<long, long>
    {
        /// <summary>
        /// 文档名
        /// </summary>
        [Column(StringLength = 50)]
        public string Name { get; set; }

        /// <summary>
        /// 显示名
        /// </summary>
        [Column(StringLength = 50)]
        public string DisplayName { get; set; }

    }
}
