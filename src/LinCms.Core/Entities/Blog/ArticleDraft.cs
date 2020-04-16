using System;
using System.Collections.Generic;
using System.Text;
using FreeSql.DataAnnotations;

namespace LinCms.Core.Entities.Blog
{
    /// <summary>
    /// 文章草稿箱
    /// </summary>
    [Table(Name = "blog_article_draft")]
    public class ArticleDraft : FullAduitEntity<Guid>
    {
        /// <summary>
        /// 正文
        /// </summary>
        [Column(DbType = "longtext")]
        public string Content { get; set; }

        public string Title { get; set; }
        /// <summary>
        ///1:MarkDown编辑器  2:富文本编辑器,
        /// </summary>
        public int Editor { get; set; } = 1;

        public virtual Article Article { get; set; }

    }
}
