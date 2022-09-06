using System;
using FreeSql.DataAnnotations;
using IGeekFan.FreeKit.Extras.AuditEntity;

namespace LinCms.Entities.Blog
{
    /// <summary>
    /// 随笔标签
    /// </summary>
    [Table(Name = "blog_tag_article")]
    public class TagArticle : Entity<Guid>
    {
        public Guid TagId { get; set; }

        public Guid ArticleId { get; set; }

        public virtual Tag Tag { get; set; }

        public virtual Article Article { get; set; }
    }
}
