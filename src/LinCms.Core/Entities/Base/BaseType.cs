
using FreeSql.DataAnnotations;

namespace LinCms.Core.Entities.Base
{
    [Table(Name = "base_type")]
    public class BaseType:FullAduitEntity
    {
         [Column(StringLength = 50)]
        public string TypeCode { get; set; }

         [Column(StringLength = 50)]
        public string FullName { get; set; }

        public int? SortCode { get; set; }
    }
}
