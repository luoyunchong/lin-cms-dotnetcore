using FreeSql.DataAnnotations;

namespace LinCms.Zero.Domain.Base
{
    [Table(Name = "base_item")]
    public class BaseItem:FullAduitEntity
    {
        public int BaseTypeId { get; set; }
        [Column(DbType = "varchar(50)")]
        public string ItemCode { get; set; }
        [Column(DbType = "varchar(50)")]
        public string ItemName { get; set; }
        public int? SortCode { get; set; }
    }
}
