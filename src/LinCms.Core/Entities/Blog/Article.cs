using System;
using System.Collections;
using System.Collections.Generic;
using FreeSql.DataAnnotations;

namespace LinCms.Core.Entities.Blog
{
    [Table(Name = "blog_article")]
    public class Article : FullAduitEntity<Guid>
    {
        /// <summary>
        /// 文章所在分类专栏Id
        /// </summary>
        public Guid? ClassifyId { get; set; }

        /// <summary>
        /// 系统内置技术频道Id
        /// </summary>
        public Guid? ChannelId { get; set; }


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
        [Column(DbType = "longtext")]
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
        /// 是否审核（默认为true),为false是代表拉黑
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
        [Column(MapType = typeof(int))]
        public ArticleType ArticleType { get; set; } = ArticleType.Original;


        /// <summary>
        ///1:MarkDown编辑器  2:富文本编辑器,
        /// </summary>
        public int Editor { get; set; } = 1;

        /// <summary>
        /// 状态：1.暂存，2.发布文章。
        /// </summary>
        public int Status { get; set; } = 1;

        /// <summary>
        /// 字数
        /// </summary>
        public long WordNumber { get; set; }

        /// <summary>
        /// 预计阅读时长
        /// </summary>
        public long ReadingTime { get; set; }

        public virtual Classify Classify { get; set; }

        public virtual Channel Channel { get; set; }


        [Navigate(nameof(CreateUserId))]
        public virtual LinUser UserInfo { get; set; }


        public virtual ICollection<Tag> Tags { get; set; }

        public virtual ArticleDraft ArticleDraft { get; set; }
        [Navigate("SubjectId")]
        public virtual ICollection<UserLike> UserLikes { get; set; }

        public void IncreaseViewHits()
        {
            this.ViewHits += 1;
        }
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
