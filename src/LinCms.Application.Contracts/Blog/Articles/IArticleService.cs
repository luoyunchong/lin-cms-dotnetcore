using System;
using System.Threading.Tasks;
using LinCms.Data;

namespace LinCms.Blog.Articles
{
    public interface IArticleService
    {
        Task<Guid> CreateAsync(CreateUpdateArticleDto createArticle);
        Task UpdateAsync(Guid id, CreateUpdateArticleDto updateArticleDto);
        Task<PagedResultDto<ArticleListDto>> GetArticleAsync(ArticleSearchDto searchDto);

        Task DeleteAsync(Guid id);

        Task<ArticleDto> GetAsync(Guid id);

        Task UpdateTagAsync(Guid id, CreateUpdateArticleDto updateArticleDto);

        Task<PagedResultDto<ArticleListDto>> GetSubscribeArticleAsync(PageDto pageDto);

        Task UpdateLikeQuantityAysnc(Guid subjectId, int likesQuantity);
        Task UpdateCommentable(Guid id, bool commetable);
    }
}
