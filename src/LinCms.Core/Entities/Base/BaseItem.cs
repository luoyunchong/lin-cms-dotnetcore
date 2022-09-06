using System;
using FreeSql.DataAnnotations;
using IGeekFan.FreeKit.Extras.AuditEntity;

namespace LinCms.Entities.Base
{
    /// <summary>
    /// 数据字典-详情项
    /// </summary>
    [Table(Name = "base_item")]
    public class BaseItem : FullAuditEntity<long, long>
    {
        public BaseItem()
        {
        }

        public BaseItem(string itemCode, string itemName, int? sortCode, bool status)
        {
            ItemCode = itemCode ?? throw new ArgumentNullException(nameof(itemCode));
            ItemName = itemName ?? throw new ArgumentNullException(nameof(itemName));
            SortCode = sortCode;
            Status = status;
        }

        public BaseItem(string itemCode, string itemName, int? sortCode, int baseTypeId, bool status) : this(itemCode, itemName, sortCode, status)
        {
            BaseTypeId = baseTypeId;
        }

        /// <summary>
        /// 数据字典-分类Id
        /// </summary>
        public long BaseTypeId { get; set; }

        /// <summary>
        /// 数据字典-编码
        /// </summary>

        [Column(StringLength = 50)]
        public string ItemCode { get; set; }

        /// <summary>
        /// 数据字典-名称
        /// </summary>

        [Column(StringLength = 50)]
        public string ItemName { get; set; }

        /// <summary>
        /// 排序码
        /// </summary>

        public int? SortCode { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }

        public virtual BaseType BaseType { get; set; }

    }
}
