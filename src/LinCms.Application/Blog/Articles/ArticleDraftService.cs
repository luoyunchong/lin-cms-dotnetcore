using System;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Blog.ArticleDrafts;
using LinCms.Entities.Blog;
using LinCms.Exceptions;
using LinCms.Security;

namespace LinCms.Blog.Articles;
/// <summary>
/// 文章草稿箱
/// </summary>
public class ArticleDraftService : ApplicationService, IArticleDraftService
{
    private readonly IAuditBaseRepository<ArticleDraft> _articleDraftRepository;
    public ArticleDraftService(IAuditBaseRepository<ArticleDraft> articleDraftRepository)
    {
        _articleDraftRepository = articleDraftRepository;
    }

    public async Task<ArticleDraftDto> GetAsync(Guid id)
    {
        ArticleDraft articleDraft = await _articleDraftRepository.Where(r => r.Id == id && r.CreateUserId == CurrentUser.FindUserId()).ToOneAsync();
        return Mapper.Map<ArticleDraftDto>(articleDraft);
    }

    public async Task UpdateAsync(Guid id, UpdateArticleDraftDto updateArticleDto)
    {
        ArticleDraft articleDraft = await _articleDraftRepository.Select.Where(r => r.Id == id).ToOneAsync();
        if (articleDraft != null && articleDraft.CreateUserId != CurrentUser.FindUserId())
        {
            throw new LinCmsException("您无权修改他人的随笔");
        }
        if (articleDraft == null)
        {
            articleDraft = new ArticleDraft { Id = id, CreateUserId = CurrentUser.FindUserId() ?? 0, CreateTime = DateTime.Now };
        }
        Mapper.Map(updateArticleDto, articleDraft);
        await _articleDraftRepository.InsertOrUpdateAsync(articleDraft);
    }
}