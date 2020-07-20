using System;
using System.Collections.Generic;
using FreeSql.DataAnnotations;

namespace LinCms.Entities.Base
{
    [Table(Name = "base_type")]
    public class BaseType : FullAduitEntity
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

        [Column(StringLength = 50)]
        public string TypeCode { get; set; }

        [Column(StringLength = 50)]
        public string FullName { get; set; }

        public int? SortCode { get; set; }

        public virtual ICollection<BaseItem> BaseItems { get; set; }
    }
}
