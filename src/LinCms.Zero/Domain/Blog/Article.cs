using System;
using System.Collections.Generic;
using FreeSql.DataAnnotations;
using LinCms.Zero.Domain.Base;

namespace LinCms.Zero.Domain.Blog
{
    [Table(Name = "blog_article")]
    public class Article : FullAduitEntity<Guid>
    {
        /// <summary>
        /// 文章所在分类专栏Id
        /// </summary>
        public Guid? ClassifyId { get; set; }

        public Classify Classify { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [Column(DbType = "varchar(200)")]
        public string Title { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>
        [Column(DbType = "varchar(400)")]
        public string Keywords { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        [Column(DbType = "varchar(400)")]
        public string Source { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        [Column(DbType = "varchar(400)")]
        public string Excerpt { get; set; }
        /// <summary>
        /// 正文
        /// </summary>
        [Column(DbType = "text")]
        public string Content { get; set; }
        public int ViewHits { get; set; }
        /// <summary>
        /// 评论数量
        /// </summary>
        public int CommentQuantity { get; set; }
        /// <summary>
        /// 点赞数量
        /// </summary>
        public int LikesQuantity { get; set; }
        /// <summary>
        /// 列表缩略图封面
        /// </summary>
        [Column(DbType = "varchar(400)")]
        public string Thumbnail { get; set; }
        /// <summary>
        /// 是否审核
        /// </summary>
        public bool IsAudit { get; set; }
        /// <summary>
        /// 是否推荐
        /// </summary>
        public bool Recommend { get; set; }
        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool IsStickie { get; set; }
        /// <summary>
        /// 随笔档案   如2019年1月
        /// </summary>
        [Column(DbType = "varchar(50)")]
        public string Archive { get; set; }
        /// <summary>
        /// 随笔类型
        /// </summary>
        [Column(DbType = "varchar(50)")]
        public ArticleType ArticleType { get; set; } = ArticleType.Original;

        /// <summary>
        /// 1:富文本编辑器,2:MarkDown编辑器
        /// </summary>
        public int Editor { get; set; } = 1;
        /// <summary>
        /// 状态：1.暂存，2.发布文章。
        /// </summary>
        public int Status { get; set; }
        /// <summary>
        /// 作者
        /// </summary>
        [Column(DbType = "varchar(50)")]
        public string Author { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }
    }

    /// <summary>
    /// 随笔类型
    /// </summary>
    public enum ArticleType
    {
        /// <summary>
        /// 原创
        /// </summary>
        Original,
        /// <summary>
        /// 转载
        /// </summary>
        Repost,
        /// <summary>
        /// 翻译
        /// </summary>
        Translated,
    }
}
