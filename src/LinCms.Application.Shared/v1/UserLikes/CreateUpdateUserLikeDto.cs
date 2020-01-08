using System;

namespace LinCms.Application.Contracts.v1.UserLikes
{
    public class CreateUpdateUserLikeDto
    {
        public Guid SubjectId { get; set; }
        /// <summary>
        /// 1.文章 2 评论
        /// </summary>
        public int SubjectType { get; set; }
    }
}
