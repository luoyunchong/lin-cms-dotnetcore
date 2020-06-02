using System;
using System.Collections.Generic;
using FreeSql.DataAnnotations;

namespace LinCms.Core.Entities.Blog
{
    /// <summary>
    /// 标签
    /// </summary>
    [Table(Name = "blog_tag")]
    public class Tag : FullAduitEntity<Guid>
    {
        /// <summary>
        /// 别名
        /// </summary>
        [Column(DbType = "varchar(200)")]
        public string Alias { get; set; }

        /// <summary>
        /// 标签封面图
        /// </summary>
        [Column(DbType = "varchar(100)")]
        public string Thumbnail { get; set; }

        /// <summary>
        /// 标签名
        /// </summary>
        [Column(DbType = "varchar(50)")]
        public string TagName { get; set; }

        /// <summary>
        /// 标签状态，true:正常，false：拉黑
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 随笔数量
        /// </summary>
        public int ArticleCount { get; set; } = 0;

        /// <summary>
        /// 浏览次数
        /// </summary>
        public int ViewHits { get; set; } = 0;

        /// <summary>
        /// 标签备注情况
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 关注数量
        /// </summary>
        public int SubscribersCount { get; set; } = 0;


        public virtual ICollection<Article> Articles { get; set; }

        public virtual ICollection<Channel> Channels { get; set; }
        [Navigate("TagId")]
        public virtual ICollection<UserTag> UserTags { get; set; }

        public virtual ICollection<ChannelTag> ChannelTags { get; set; }

        [Navigate("CreateUserId")]
        public virtual LinUser LinUser { get; set; }

    }
}
