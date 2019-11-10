using System;
using System.Collections.Generic;
using FreeSql.DataAnnotations;

namespace LinCms.Zero.Domain.Blog
{
    /// <summary>
    /// 用户评论信息
    /// </summary>
    [Table(Name = "blog_comment")]
    public class Comment : FullAduitEntity<Guid>
    {
        /// <summary>
        /// 回复评论Id
        /// </summary>
        public Guid? RespId { get; set; }
        /// <summary>
        /// 根回复id
        /// </summary>
        public Guid? RootCommentId { get; set; }

        /// <summary>
        /// 被回复的用户Id
        /// </summary>
        public long? RespUserId { get; set; }
        /// <summary>
        /// 回复的文本内容
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 点赞量
        /// </summary>
        public int LikesQuantity { get; set; }
        /// <summary>
        /// 是否已审核
        /// </summary>
        public bool? IsAudited { get; set; } = true;

        /// <summary>
        /// 关联随笔id
        /// </summary>
        public Guid? SubjectId { get; set; }

        /// <summary>
        /// 评论的对象 1 是随笔，其他为以后扩展
        /// </summary>
        public int SubjectType { get; set; }

        /// <summary>
        /// 评论的用户-OneToOne
        /// </summary>
        [Navigate("CreateUserId")]
        public LinUser UserInfo { get; set; }
        /// <summary>
        /// 被回复的用户-OneToOne
        /// </summary>
        [Navigate("RespUserId")]
        public LinUser RespUserInfo { get; set; }


        [Navigate("RootCommentId")]
        public ICollection<Comment> Childs { get; set; }

        [Navigate("RespId")]
        public Comment Parent { get; set; }
    }

}
