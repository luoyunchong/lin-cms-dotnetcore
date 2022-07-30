using System;
using FreeSql.DataAnnotations;

namespace LinCms.Entities.Base
{
    [Table(Name = "base_item")]
    public class BaseItem : FullAuditEntity
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

        public long BaseTypeId { get; set; }

        [Column(StringLength = 50)]
        public string ItemCode { get; set; }

        [Column(StringLength = 50)]
        public string ItemName { get; set; }

        public int? SortCode { get; set; }

        public bool Status { get; set; }

        public virtual BaseType BaseType { get; set; }

    }
}
