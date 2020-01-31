
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.Blog.Comments
{
    public interface ICommentService
    {
        void Delete(Comment comment);
    }
}
