using System;
using FreeSql.DataAnnotations;

namespace LinCms.Zero.Domain.Blog
{
    [Table(Name = "blog_tag_article")]
    public class TagArticle  :Entity<Guid>
    {
        public Guid TagId { get; set; }
        public Guid ArticleId { get; set; }

        public Tag Tag { get; set; }
        public Article Article { get; set; }

    }
}
