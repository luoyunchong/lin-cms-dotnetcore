using System;
using System.Collections.Generic;
using System.Text;
using FreeSql.DataAnnotations;

namespace LinCms.Core.Entities
{
    [Table(DisableSyncStructure = true,Name = "app_serilog")]
    public class SerilogDO
    {
       public long Id { get; set; }
       public string Exception { get; set; }
       public int Level { get; set; }
       public string Message { get; set; }
       public string MessageTemplate { get; set; }
       public string Properties { get; set; }
       public DateTime Timestamp { get; set; }
    }
}
