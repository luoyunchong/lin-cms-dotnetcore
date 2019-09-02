using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace LinCms.Zero.Domain.Blog
{
    /// <summary>
    /// 留言板
    /// </summary>
    [Table(Name = "blog_reply")]
    public class Reply : FullAduitEntity
    {
        public int? PId { get; set; }
        /// <summary>
        /// @的用户名，用于前台的显示效果
        /// </summary>
        [Column(DbType = "varchar(50)")]
        public string PName { get; set; }
        public string Text { get; set; }
        [Column(DbType = "varchar(50)")]
        public string Ip { get; set; }
        [Column(DbType = "varchar(50)")]
        public string Agent { get; set; }
        //系统
        [Column(DbType = "varchar(50)")]
        public string System { get; set; }
        /// <summary>
        /// 地理位置
        /// </summary>
        [Column(DbType = "varchar(50)")]
        public string GeoPosition { get; set; }

        [Column(DbType = "varchar(50)")]
        public string Qq { get; set; }
        [Column(DbType = "varchar(50)")]
        public string AuName { get; set; }
        [Column(DbType = "varchar(400)")]
        public string PersonalWebsite { get; set; }
        [Column(DbType = "varchar(50)")]
        public string AuEmail { get; set; }
        [Column(DbType = "varchar(50)")]
        public string UserHost { get; set; }
        /// <summary>
        /// 是否已审核
        /// </summary>
        public bool? IsAduit { get; set; }
        /// <summary>
        /// 评论人的头像，如果未登录状态下，则随机生成一个头像地址。已登录状态下，取用户表的头像地址
        /// </summary>
        [Column(DbType = "varchar(500)")]
        public string Avatar { get; set; }
    }

    [Table(Name = "blog_review")]
    public class Review : Reply
    {
        /// <summary>
        /// 关于文章id
        /// </summary>
        public int? ArticleId { get; set; }
    }

}
