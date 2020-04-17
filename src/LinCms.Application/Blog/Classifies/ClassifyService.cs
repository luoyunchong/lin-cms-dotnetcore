using System;
using System.Threading.Tasks;
using LinCms.Application.Contracts.Blog.Classifys;
using LinCms.Core.Entities.Blog;
using LinCms.Core.IRepositories;

namespace LinCms.Application.Blog.Classifies
{
    public class ClassifyService : IClassifyService
    {
        private readonly IAuditBaseRepository<Classify> _classifyBaseRepository;

        public ClassifyService(IAuditBaseRepository<Classify> classifyBaseRepository)
        {
            _classifyBaseRepository = classifyBaseRepository;
        }

        public async Task UpdateArticleCountAsync(Guid? id, int inCreaseCount)
        {
            if (id == null)
            {
                return;
            }
            //防止数量一直减，减到小于0
            if (inCreaseCount < 0)
            {
                Classify classify = await _classifyBaseRepository.Select.Where(r => r.Id == id).ToOneAsync();
                if (classify.ArticleCount < -inCreaseCount)
                {
                    return;
                }
            }

            await _classifyBaseRepository.UpdateDiy.Set(r => r.ArticleCount + inCreaseCount).Where(r => r.Id == id)
                            .ExecuteAffrowsAsync();
        }
    }
}
