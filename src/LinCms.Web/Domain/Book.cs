using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSql.DataAnnotations;
using Newtonsoft.Json;

namespace LinCms.Web.Domain
{
    [JsonObject(MemberSerialization.OptIn), Table(Name = "book")]
    public class Book : FullAduitEntity
    {
        [Column(DbType = "varchar(30)")]
        public string Author { get; set; } = string.Empty;

        [Column(DbType = "varchar(50)")]
        public string Image { get; set; } = string.Empty;

        [Column(DbType = "varchar(1000)")]
        public string Summary { get; set; } = string.Empty;

        [Column(DbType = "varchar(50)")]
        public string Title { get; set; } = string.Empty;

    }

}
