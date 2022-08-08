using System;
using System.Collections.Generic;
using FreeSql.DataAnnotations;

namespace LinCms.Entities.Base
{
    /// <summary>
    /// 数据字典-分类
    /// </summary>
    [Table(Name = "base_type")]
    public class BaseType : FullAuditEntity
    {
        public BaseType()
        {
        }

        public BaseType(string typeCode, string fullName, int? sortCode)
        {
            TypeCode = typeCode ?? throw new ArgumentNullException(nameof(typeCode));
            FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
            SortCode = sortCode;
        }

        /// <summary>
        /// 分类编码
        /// </summary>
        [Column(StringLength = 50)]
        public string TypeCode { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        [Column(StringLength = 50)]
        public string FullName { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>
        public int? SortCode { get; set; }

        public virtual ICollection<BaseItem> BaseItems { get; set; }
    }
}
