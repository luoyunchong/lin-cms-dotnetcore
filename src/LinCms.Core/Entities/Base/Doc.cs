using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinCms.Entities.Base
{
    /// <summary>
    /// 文档
    /// </summary>
    public class Doc : FullAduitEntity
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
