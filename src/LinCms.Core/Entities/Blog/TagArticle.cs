using System;
using FreeSql.DataAnnotations;

namespace LinCms.Entities.Blog
{
    [Table(Name = "blog_tag_article")]
    public class TagArticle : Entity<Guid>
    {
        public Guid TagId { get; set; }

        public Guid ArticleId { get; set; }

        public virtual Tag Tag { get; set; }

        public virtual Article Article { get; set; }
    }
}
