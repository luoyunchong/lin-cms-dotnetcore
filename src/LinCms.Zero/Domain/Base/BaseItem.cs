using FreeSql.DataAnnotations;

namespace LinCms.Zero.Domain.Base
{
    [Table(Name = "base_item")]
    public class BaseItem:FullAduitEntity
    {
        public int BaseTypeId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int? SortCode { get; set; }
    }
}
