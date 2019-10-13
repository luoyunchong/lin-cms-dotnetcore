using System;
using System.Collections.Generic;
using System.Text;
using FreeSql.DataAnnotations;

namespace LinCms.Zero.Domain.Blog
{
    [Table(Name = "blog_classify")]
   public class Classify:FullAduitEntity
    {
        /// <summary>
        /// 分类编码
        /// </summary>
        [Column(DbType = "varchar(50)")]
        public string ClassifyCode { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortCode { get; set; }
        /// <summary>
        /// 分类专栏名称
        /// </summary>
        [Column(DbType = "varchar(50)")]
        public string ClassifyName { get; set; }

        public List<Article> Articles { get; set; }
    }
}
