using System;
using LinCms.Data;

namespace LinCms.Blog.Comments;

public class CommentSearchDto : PageDto
{
    public Guid? RootCommentId { get; set; }
    public Guid? SubjectId { get; set; }

    public String Text { get; set; }
}