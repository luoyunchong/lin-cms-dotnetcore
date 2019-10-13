using FreeSql;
using LinCms.Web.Services.Interfaces;
using LinCms.Zero.Domain.Blog;
using LinCms.Zero.Repositories;

namespace LinCms.Web.Services
{
    public class ArticleAppService:IArticleService
    {
        private readonly AuditBaseRepository<Article> _articleRepository;
        private readonly GuidRepository<TagArticle> _tagArticleRepository;

        public ArticleAppService(AuditBaseRepository<Article> articleRepository, GuidRepository<TagArticle> tagArticleRepository)
        {
            _articleRepository = articleRepository;
            _tagArticleRepository = tagArticleRepository;
        }

        public void Delete(int id)
        {
            _tagArticleRepository.Delete(r => r.ArticleId == id);
            _articleRepository.Delete(new Article { Id = id });
        }
    }
}
