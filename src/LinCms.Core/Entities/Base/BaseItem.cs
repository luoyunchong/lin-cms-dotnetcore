using System;
using FreeSql.DataAnnotations;

namespace LinCms.Entities.Base
{
    [Table(Name = "base_item")]
    public class BaseItem : FullAduitEntity
    {
        public BaseItem()
        {
        }

        public BaseItem(string itemCode, string itemName, int? sortCode)
        {
            ItemCode = itemCode ?? throw new ArgumentNullException(nameof(itemCode));
            ItemName = itemName ?? throw new ArgumentNullException(nameof(itemName));
            SortCode = sortCode;
        }

        public BaseItem(string itemCode, string itemName, int? sortCode, int baseTypeId) : this(itemCode, itemName, sortCode)
        {
            BaseTypeId = baseTypeId;
        }

        public long BaseTypeId { get; set; }

        [Column(StringLength = 50)]
        public string ItemCode { get; set; }

        [Column(StringLength = 50)]
        public string ItemName { get; set; }

        public int? SortCode { get; set; }

        public virtual BaseType BaseType { get; set; }

    }
}
