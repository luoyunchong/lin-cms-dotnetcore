using LinCms.Zero.Data;
using System;

namespace LinCms.Web.Models.v1.Comments
{
    public class CommentSearchDto:PageDto
    {
        public Guid? RootCommentId { get; set; }
        public Guid? SubjectId { get; set; }

        public String Text { get; set; }
    }
}
