using System;
using LinCms.Zero.Domain;

namespace LinCms.Web.Models.v1.BaseItems
{
    public class BaseItemDto: Entity
    {
        public int BaseTypeId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int? SortCode { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
