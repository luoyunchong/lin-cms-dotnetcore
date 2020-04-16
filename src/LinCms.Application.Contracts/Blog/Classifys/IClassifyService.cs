using System;
using System.Threading.Tasks;

namespace LinCms.Application.Contracts.Blog.Classifys
{
    public interface IClassifyService
    {
        Task UpdateArticleCountAsync(Guid? id, int inCreaseCount);
    }
}
