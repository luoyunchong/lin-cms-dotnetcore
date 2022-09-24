using System;
using System.Collections.Generic;
using FreeSql.DataAnnotations;
using IGeekFan.FreeKit.Extras.AuditEntity;

namespace LinCms.Entities.Blog
{

    /// <summary>
    /// 随笔专栏，由普通用户创建
    /// </summary>
    [Table(Name = "blog_classify")]
    public class Classify : FullAuditEntity<Guid, long>
    {
        /// <summary>
        /// 封面图
        /// </summary>
        [Column(StringLength = 100)]
        public string Thumbnail { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int SortCode { get; set; }
        /// <summary>
        /// 分类专栏名称
        /// </summary>
        [Column(StringLength = 50)]
        public string ClassifyName { get; set; }

        /// <summary>
        /// 随笔数量
        /// </summary>
        public int ArticleCount { get; set; }

        public virtual List<Article> Articles { get; set; }

        public void ReduceArticleCount()
        {
            ArticleCount -= 1;
        }

        public void IncreaseArticleCount()
        {
            ArticleCount += 1;
        }

        public void UpdateArticleCount(int inCreaseCount)
        {
            //防止数量一直减，减到小于0
            if (inCreaseCount < 0)
            {
                if (ArticleCount < -inCreaseCount)
                {
                    return;
                }
            }
            ArticleCount += inCreaseCount;
        }
    }
}
