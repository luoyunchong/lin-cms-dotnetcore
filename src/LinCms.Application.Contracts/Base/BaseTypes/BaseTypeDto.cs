using System;
using LinCms.Entities;

namespace LinCms.Base.BaseTypes
{
    public class BaseTypeDto : EntityDto
    {
        public string TypeCode { get; set; }
        public string FullName { get; set; }
        public int? SortCode { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
