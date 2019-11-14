
using LinCms.Zero.Domain.Blog;

namespace LinCms.Web.Services.v1.Interfaces
{
    public interface ICommentService
    {
        void Delete(Comment comment);
    }
}
