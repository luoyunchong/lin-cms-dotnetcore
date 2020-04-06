
using System;

namespace LinCms.Application.Contracts.Blog.Comments.Dtos
{
    public class CreateCommentDto
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
        /// 回复的文本内容
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 关联随笔id
        /// </summary>
        public Guid? SubjectId { get; set; }
        /// <summary>
        /// 被回复的用户Id
        /// </summary>
        public long RespUserId { get; set; }

        public int SubjectType { get; set; }
    }
}
