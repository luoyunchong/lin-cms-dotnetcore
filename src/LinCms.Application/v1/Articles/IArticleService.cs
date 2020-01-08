using System;
using LinCms.Application.Contracts.v1.Articles;
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.v1.Articles
{
    public interface IArticleService
    {
         void Delete(Guid id);

         ArticleDto Get(Guid id);

         void CreateArticle(CreateUpdateArticleDto createArticle);

         void UpdateArticle(CreateUpdateArticleDto updateArticleDto, Article article);
    }
}
