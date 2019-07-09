using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSql.DataAnnotations;
using Newtonsoft.Json;

namespace LinCms.Web.Domain
{
    [Table(Name = "lin_poem")]
    public class LinPoem:FullAduitEntity
    {

        /// <summary>
        /// 作者
        /// </summary>
        [Column(DbType = "varchar(50)")]
        public string Author { get; set; } = string.Empty;

        /// <summary>
        /// 内容，以/来分割每一句，以|来分割宋词的上下片
        /// </summary>
        [Column( DbType = "text")]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 朝代
        /// </summary>
        [Column(DbType = "varchar(50)")]
        public string Dynasty { get; set; } = string.Empty;

        /// <summary>
        /// 配图
        /// </summary>
        public string Image { get; set; } = string.Empty;

        /// <summary>
        /// 标题
        /// </summary>
        [Column(DbType = "varchar(50)")]
        public string Title { get; set; } = string.Empty;


    }
}
