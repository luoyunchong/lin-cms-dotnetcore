using System;
using System.Collections.Generic;
using FreeSql.DataAnnotations;

namespace LinCms.Core.Entities.Blog
{
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
        public bool Status { get; set; }
        /// <summary>
        /// 随笔数量
        /// </summary>
        public int ArticleCount { get; set; } = 0;

        /// <summary>
        /// 关注数量
        /// </summary>
        public int SubscribersCount { get; set; } = 0;
        public virtual ICollection<Article> Articles { get; set; }
        public virtual ICollection<UserTag> UserTags { get; set; }

        [Navigate("CreateUserId")]
        public virtual LinUser LinUser { get; set; }

    }
}
