using System;
using System.Collections.Generic;
using FreeSql.DataAnnotations;

namespace LinCms.Core.Entities.Blog
{

    /// <summary>
    /// 文章专栏，由普通用户创建
    /// </summary>
    [Table(Name = "blog_classify")]
   public class Classify:FullAduitEntity<Guid>
    {
        /// <summary>
        /// 封面图
        /// </summary>
        [Column(DbType = "varchar(100)")]
        public string Thumbnail { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortCode { get; set; }
        /// <summary>
        /// 分类专栏名称
        /// </summary>
        [Column(DbType = "varchar(50)")]
        public string ClassifyName { get; set; }

        /// <summary>
        /// 随笔数量
        /// </summary>
        public int ArticleCount { get; set; } = 0;

        public virtual List<Article> Articles { get; set; }

        public void ReduceArticleCount()
        {
            this.ArticleCount -= 1;
        }

        public void IncreaseArticleCount()
        {
            this.ArticleCount += 1;
        }

        public void UpdateDiyArticleCount(int articleCount)
        {
            this.ArticleCount += articleCount;
        }
    }
}
