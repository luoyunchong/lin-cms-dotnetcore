using System;
using LinCms.Core.Entities;

namespace LinCms.Application.Contracts.Base.BaseItems.Dtos
{
    public class BaseItemDto: EntityDto
    {
        public int BaseTypeId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int? SortCode { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
