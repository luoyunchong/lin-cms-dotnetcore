using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSql.DataAnnotations;
using Newtonsoft.Json;

namespace LinCms.Web.Domain
{
    [Table(Name = "lin_file")]
    public class LinFile : FullAduitEntity
    {
        /// <summary>
        /// 后缀
        /// </summary>
        [Column(DbType = "varchar(50)")]
        public string Extension { get; set; } = string.Empty;

        /// <summary>
        /// 图片md5值，防止上传重复图片
        /// </summary>
        [Column(DbType = "varchar(40)")]
        public string Md5 { get; set; } = string.Empty;

        /// <summary>
        /// 名称
        /// </summary>
        [Column(DbType = "varchar(100)")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 路径
        /// </summary>
        [Column(DbType = "varchar(500)")]
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// 大小
        /// </summary>
        public int? Size { get; set; }

        /// <summary>
        /// 1 local，其他表示其他地方
        /// </summary>
        public short? Type { get; set; }


    }
}
