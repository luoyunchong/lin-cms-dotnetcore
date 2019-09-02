using FreeSql.DataAnnotations;

namespace LinCms.Zero.Domain.Base
{
    [Table(Name = "base_type")]
    public class BaseType:FullAduitEntity
    {
        public string TypeCode { get; set; }
        public string FullName { get; set; }
        public int? SortCode { get; set; }
    }
}
