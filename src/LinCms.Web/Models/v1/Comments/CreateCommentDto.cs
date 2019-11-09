
using System;

namespace LinCms.Web.Models.v1.Comments
{
    public class CreateCommentDto
    {
        /// <summary>
        /// 回复的父Id
        /// </summary>
        public Guid? PId { get; set; }
        /// <summary>
        /// @的用户昵称
        /// </summary>
        public string PName { get; set; }
        /// <summary>
        /// 回复的文本内容
        /// </summary>
        public string Text { get; set; }
        
        public string AuName { get; set; }
        public string AuEmail { get; set; }
        /// <summary>
        /// 关联随笔id
        /// </summary>
        public Guid? ArticleId { get; set; }
    }
}
