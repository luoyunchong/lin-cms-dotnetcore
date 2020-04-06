using System;
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.Contracts.Blog.UserLikes.Dtos
{
    public class CreateUpdateUserLikeDto
    {
        public Guid SubjectId { get; set; }
        /// <summary>
        /// 1.文章 2 评论
        /// </summary>
        public UserLikeSubjectType SubjectType { get; set; }
    }
}
