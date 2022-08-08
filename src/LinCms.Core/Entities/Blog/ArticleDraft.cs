using System;
using FreeSql.DataAnnotations;

namespace LinCms.Entities.Blog
{
    /// <summary>
    /// 随笔草稿箱
    /// </summary>
    [Table(Name = "blog_article_draft")]
    public class ArticleDraft : FullAuditEntity<Guid>
    {
        public ArticleDraft()
        {
        }

        public ArticleDraft(Guid id, string content, string title, int editor)
        {
            Id = id;
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Editor = editor;
        }

        /// <summary>
        /// 正文
        /// </summary>
        [Column(StringLength = -2)]
        public string Content { get; set; }

        [Column(StringLength = 200)]
        public string Title { get; set; }

        /// <summary>
        ///1:MarkDown编辑器  2:富文本编辑器,
        /// </summary>
        public int Editor { get; set; } = 1;

        public virtual Article Article { get; set; }

    }
}
