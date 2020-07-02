
using FreeSql.DataAnnotations;

namespace LinCms.Core.Entities.Base
{
    [Table(Name = "base_item")]
    public class BaseItem:FullAduitEntity
    {
        public int BaseTypeId { get; set; }

         [Column(StringLength = 50)]
        public string ItemCode { get; set; }

         [Column(StringLength = 50)]
        public string ItemName { get; set; }

        public int? SortCode { get; set; }

    }
}
