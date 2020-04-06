using System;

namespace LinCms.Application.Contracts.Blog.Classifys
{
    public interface IClassifyService
    {
        void UpdateArticleCount(Guid? id, int inCreaseCount);
    }
}
