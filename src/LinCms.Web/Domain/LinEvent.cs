using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSql.DataAnnotations;
using Newtonsoft.Json;

namespace LinCms.Web.Domain
{
    [Table(Name = "lin_event")]
    public class LinEvent : Entity
    {
        /// <summary>
        /// 所属权限组id
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// 信息
        /// </summary>
        [Column(DbType = "varchar(250)")]
        public string MessageEvents { get; set; } = string.Empty;

    }
}
