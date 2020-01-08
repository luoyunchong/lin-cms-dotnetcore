using System;
using LinCms.Core.Entities.Blog;
using LinCms.Infrastructure.Repositories;

namespace LinCms.Application.v1.Classifies
{
    public class ClassifyService : IClassifyService
    {
        private readonly AuditBaseRepository<Classify> _classifyBaseRepository;

        public ClassifyService(AuditBaseRepository<Classify> classifyBaseRepository)
        {
            _classifyBaseRepository = classifyBaseRepository;
        }

        public void UpdateArticleCount(Guid? id, int inCreaseCount)
        {
            if (id == null)
            {
                return;
            }
            //防止数量一直减，减到小于0
            if (inCreaseCount < 0)
            {
                Classify classify = _classifyBaseRepository.Select.Where(r => r.Id == id).ToOne();
                if (classify.ArticleCount < -inCreaseCount)
                {
                    return;
                }
            }

            _classifyBaseRepository.UpdateDiy.Set(r => r.ArticleCount + inCreaseCount).Where(r => r.Id == id)
                .ExecuteAffrows();
        }
    }
}
