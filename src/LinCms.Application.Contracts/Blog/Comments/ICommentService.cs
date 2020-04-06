
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.Contracts.Blog.Comments
{
    public interface ICommentService
    {
        void Delete(Comment comment);
    }
}
