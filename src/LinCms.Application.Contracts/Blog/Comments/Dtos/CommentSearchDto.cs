using System;
using LinCms.Core.Data;

namespace LinCms.Application.Contracts.Blog.Comments.Dtos
{
    public class CommentSearchDto:PageDto
    {
        public Guid? RootCommentId { get; set; }
        public Guid? SubjectId { get; set; }

        public String Text { get; set; }
    }
}
