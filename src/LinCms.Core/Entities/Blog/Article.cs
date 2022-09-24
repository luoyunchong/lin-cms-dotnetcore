using System;
using System.Collections.Generic;
using FreeSql.DataAnnotations;
using IGeekFan.FreeKit.Extras.AuditEntity;
using LinCms.Exceptions;

namespace LinCms.Entities.Blog
{
    /// <summary>
    /// 随笔
    /// </summary>
    [Table(Name = "blog_article")]
    public class Article : FullAuditEntity<Guid, long>
    {
        /// <summary>
        /// 随笔所在分类专栏Id
        /// </summary>
        public Guid? ClassifyId { get; set; }

        /// <summary>
        /// 系统内置技术频道Id
        /// </summary>
        public Guid ChannelId { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Column(StringLength = 200)]
        public string Title { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        [Column(StringLength = 400)]
        public string Keywords { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        [Column(StringLength = 400)]
        public string Source { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        [Column(StringLength = 400)]
        public string Excerpt { get; set; }

        /// <summary>
        /// 正文
        /// </summary>
        [Column(StringLength = -2)]
        public string Content { get; set; }

        /// <summary>
        /// 浏览量
        /// </summary>
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
        [Column(StringLength = 400)]
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
        [Column(StringLength = 50)]
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
        /// 状态：1.暂存;2.发布;3.私密随笔
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

        /// <summary>
        /// 随笔是否开启评论
        /// </summary>
        public bool Commentable { get; set; } = true;

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
            ViewHits += 1;
        }

        public void UpdateLikeQuantity(int likesQuantity)
        {
            if (IsAudit == false)
            {
                throw new LinCmsException("该随笔因违规被拉黑");
            }

            if (likesQuantity < 0)
            {
                //防止数量一直减，减到小于0
                if (LikesQuantity < -likesQuantity)
                {
                    return;
                }
            }

            LikesQuantity += likesQuantity;
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
