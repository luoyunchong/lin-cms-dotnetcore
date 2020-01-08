
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.v1.Comments
{
    public interface ICommentService
    {
        void Delete(Comment comment);
    }
}
