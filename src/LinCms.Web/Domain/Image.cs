using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSql.DataAnnotations;
using Newtonsoft.Json;

namespace LinCms.Web.Domain
{
    [Table(Name = "image")]
    public class Image:Entity
    {
        public int From { get; set; }

        public string Url { get; set; } = string.Empty;

    }
}
