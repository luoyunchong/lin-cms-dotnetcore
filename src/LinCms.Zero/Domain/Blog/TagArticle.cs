using System;
using System.Collections.Generic;
using System.Text;
using FreeSql.DataAnnotations;

namespace LinCms.Zero.Domain.Blog
{
    [Table(Name = "blog_tag_article")]
    public class TagArticle  :Entity
    {
        public int TagId { get; set; }
        public int ArticleId { get; set; }
    }
}
