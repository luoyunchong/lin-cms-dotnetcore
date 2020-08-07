using System;
using System.Threading.Tasks;
using LinCms.Blog.ArticleDrafts;
using LinCms.Entities.Blog;
using LinCms.Exceptions;
using LinCms.IRepositories;

namespace LinCms.Blog.Articles
{
    public class ArticleDraftService : ApplicationService, IArticleDraftService
    {
        private readonly IAuditBaseRepository<ArticleDraft> _articleDraftRepository;
        public ArticleDraftService(IAuditBaseRepository<ArticleDraft> articleDraftRepository)
        {
            _articleDraftRepository = articleDraftRepository;
        }

        public async Task<ArticleDraftDto> GetAsync(Guid id)
        {
            ArticleDraft articleDraft = await _articleDraftRepository.Where(r => r.Id == id && r.CreateUserId == CurrentUser.Id).ToOneAsync();
            return Mapper.Map<ArticleDraftDto>(articleDraft);
        }

        public async Task UpdateAsync(Guid id, UpdateArticleDraftDto updateArticleDto)
        {
            ArticleDraft articleDraft = await _articleDraftRepository.Select.Where(r => r.Id == id).ToOneAsync();
            if (articleDraft != null && articleDraft.CreateUserId != CurrentUser.Id)
            {
                throw new LinCmsException("您无权修改他人的随笔");
            }
            if (articleDraft == null)
            {
                articleDraft = new ArticleDraft { Id = id, CreateUserId = CurrentUser.Id ?? 0, CreateTime = DateTime.Now };
            }
            Mapper.Map(updateArticleDto, articleDraft);
            await _articleDraftRepository.InsertOrUpdateAsync(articleDraft);
        }
    }
}
