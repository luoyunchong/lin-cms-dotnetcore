using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Zero.Domain;

namespace LinCms.Web.Models.v1.BaseItems
{
    public class BaseItemDto: Entity
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int? SortCode { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
