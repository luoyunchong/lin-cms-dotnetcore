using System;

namespace LinCms.Application.Blog.Classifies
{
    public interface IClassifyService
    {
        void UpdateArticleCount(Guid? id, int inCreaseCount);
    }
}
