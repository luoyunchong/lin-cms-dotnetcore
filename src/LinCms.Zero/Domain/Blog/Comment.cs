using FreeSql.DataAnnotations;

namespace LinCms.Zero.Domain.Blog
{
    /// <summary>
    /// 用户评论信息
    /// </summary>
    [Table(Name = "blog_comment")]
    public class Comment : FullAduitEntity
    {
        /// <summary>
        /// 回复的父Id
        /// </summary>
        public int? PId { get; set; }
        /// <summary>
        /// @的用户名，用于前台的显示效果
        /// </summary>
        [Column(DbType = "varchar(50)")]
        public string PName { get; set; }
        /// <summary>
        /// 回复的文本内容
        /// </summary>
        public string Text { get; set; }


        /// <summary>
        /// 用户昵称
        /// </summary>
        [Column(DbType = "varchar(50)")]
        public string AuName { get; set; }
        /// <summary>
        /// 用户邮件
        /// </summary>
        [Column(DbType = "varchar(50)")]
        public string AuEmail { get; set; }


        [Column(DbType = "varchar(50)")]
        public string Ip { get; set; }
        /// <summary>
        /// User-Agent浏览器
        /// </summary>
        [Column(DbType = "varchar(200)")]
        public string Agent { get; set; }
        /// <summary>
        /// 操作系统
        /// </summary>
        [Column(DbType = "varchar(50)")]
        public string System { get; set; }
        /// <summary>
        /// 地理位置 运营商
        /// </summary>
        [Column(DbType = "varchar(50)")]
        public string GeoPosition { get; set; }
        /// <summary>
        /// 主机名
        /// </summary>
        [Column(DbType = "varchar(50)")]
        public string UserHost { get; set; }

        /// <summary>
        /// 是否已审核
        /// </summary>
        public bool? IsAudited { get; set; } = true;
        /// <summary>
        /// 评论人的头像，如果未登录状态下，则随机生成一个头像地址。已登录状态下，取用户表的头像地址
        /// </summary>
        [Column(DbType = "varchar(500)")]
        public string Avatar { get; set; }


        /// <summary>
        /// 关联随笔id
        /// </summary>
        public int? ArticleId { get; set; }
    }

}
