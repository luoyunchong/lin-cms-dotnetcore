using FreeSql.DataAnnotations;

namespace LinCms.Zero.Domain.Blog
{
    [Table(Name = "blog_tag_article")]
    public class TagArticle  :Entity
    {
        public int TagId { get; set; }
        public int ArticleId { get; set; }

        public Tag Tag { get; set; }
        public Article Article { get; set; }

    }
}
