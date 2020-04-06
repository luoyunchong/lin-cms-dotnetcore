using System;
using System.Threading.Tasks;
using LinCms.Application.Contracts.Blog.Articles.Dtos;
using LinCms.Core.Data;
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.Contracts.Blog.Articles
{
    public interface IArticleService
    {
        Task<PagedResultDto<ArticleListDto>> GetArticleAsync(ArticleSearchDto searchDto);

        void Delete(Guid id);

        Task<ArticleDto> GetAsync(Guid id);

        Task CreateAsync(CreateUpdateArticleDto createArticle);

        Task UpdateAsync(CreateUpdateArticleDto updateArticleDto, Article article);


        PagedResultDto<ArticleListDto> GetSubscribeArticle(PageDto pageDto);
    }
}
