using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Zero.Domain;

namespace LinCms.Web.Models.v1.BaseTypes
{
    public class BaseTypeDto :Entity
    {
        public string TypeCode { get; set; }
        public string FullName { get; set; }
        public int? SortCode { get; set; }
    }
}
