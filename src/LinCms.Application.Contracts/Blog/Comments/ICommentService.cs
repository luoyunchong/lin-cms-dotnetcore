
using System;
using System.Threading.Tasks;
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.Contracts.Blog.Comments
{
    public interface ICommentService
    {
        Task DeleteAsync(Comment comment);

        Task UpdateLikeQuantity(Guid subjectId, int likesQuantity);
    }
}
