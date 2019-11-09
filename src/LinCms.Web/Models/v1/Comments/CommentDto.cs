using System;
using LinCms.Zero.Domain;

namespace LinCms.Web.Models.v1.Comments
{
    public class CommentDto:EntityDto<Guid>,ICreateAduitEntity
    {
        /// <summary>
        /// 回复的父Id
        /// </summary>
        public Guid? PId { get; set; }
        /// <summary>
        /// @的用户名，用于前台的显示效果
        /// </summary>
        public string PName { get; set; }
        /// <summary>
        /// 回复的文本内容
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string AuName { get; set; }
        /// <summary>
        /// 用户邮件
        /// </summary>
        public string AuEmail { get; set; }

        public string Ip { get; set; }
        /// <summary>
        /// User-Agent浏览器
        /// </summary>
        public string Agent { get; set; }
        /// <summary>
        /// 操作系统
        /// </summary>
        public string System { get; set; }
        /// <summary>
        /// 地理位置 运营商
        /// </summary>
        public string GeoPosition { get; set; }
        /// <summary>
        /// 主机名
        /// </summary>
        public string UserHost { get; set; }

        /// <summary>
        /// 评论人的头像，如果未登录状态下，则随机生成一个头像地址。已登录状态下，取用户表的头像地址
        /// </summary>
        public string Avatar { get; set; }


        /// <summary>
        /// 关联随笔id
        /// </summary>
        public Guid? ArticleId { get; set; }

        public long? CreateUserId { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
