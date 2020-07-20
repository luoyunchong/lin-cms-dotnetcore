using System;
using LinCms.Entities;

namespace LinCms.Base.BaseItems
{
    public class BaseItemDto : EntityDto
    {
        public int BaseTypeId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int? SortCode { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
