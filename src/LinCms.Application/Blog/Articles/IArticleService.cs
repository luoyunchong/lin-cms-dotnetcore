using System;
using LinCms.Application.Contracts.Blog.Articles;
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.Blog.Articles
{
    public interface IArticleService
    {
         void Delete(Guid id);

         ArticleDto Get(Guid id);

         void CreateArticle(CreateUpdateArticleDto createArticle);

         void UpdateArticle(CreateUpdateArticleDto updateArticleDto, Article article);
    }
}
