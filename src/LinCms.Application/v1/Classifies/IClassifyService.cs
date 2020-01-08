using System;

namespace LinCms.Application.v1.Classifies
{
    public interface IClassifyService
    {
        void UpdateArticleCount(Guid? id, int inCreaseCount);
    }
}
