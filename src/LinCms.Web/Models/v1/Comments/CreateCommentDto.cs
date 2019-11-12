
using System;

namespace LinCms.Web.Models.v1.Comments
{
    public class CreateCommentDto
    {
        /// <summary>
        /// 回复评论Id
        /// </summary>
        public Guid? RespId { get; set; }
        /// <summary>
        /// 回复的文本内容
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 关联随笔id
        /// </summary>
        public Guid? SubjectId { get; set; }
    }
}
