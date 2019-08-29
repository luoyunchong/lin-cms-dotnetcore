using FreeSql.DataAnnotations;

namespace LinCms.Zero.Domain.Blog
{
    [Table(Name = "blog_article")]
    public class Article : FullAduitEntity
    {
        public int? FId { get; set; }
        [Column(DbType = "varchar(200)")]
        public string Title { get; set; }
        [Column(DbType = "varchar(400)")]
        public string Keywords { get; set; }
        [Column(DbType = "varchar(400)")]
        public string Source { get; set; }
        [Column(DbType = "varchar(400)")]
        public string Excerpt { get; set; }
        public string Content { get; set; }
        public int ViewHits { get; set; }
        public int CommentQuantity { get; set; }
        public int PointQuantity { get; set; }
        [Column(DbType = "varchar(400)")]
        public string Thumbnail { get; set; }
        public bool IsAudit { get; set; }
        public bool Recommend { get; set; }
        public bool IsStickie { get; set; }
        /// <summary>
        /// 随笔档案   如2019年1月
        /// </summary>
        [Column(DbType = "varchar(50)")]
        public string Archive { get; set; }

        [Column(DbType = "varchar(50)")]
        public string ArticleType { get; set; }

        /// <summary>
        /// 1:富文本编辑器,2:MarkDown编辑器
        /// </summary>
        public int Editor { get; set; } = 1;
    }
}
