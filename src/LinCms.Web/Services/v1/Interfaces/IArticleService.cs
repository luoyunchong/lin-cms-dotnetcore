using System;
using System.Collections.Generic;
using LinCms.Web.Models.v1.Articles;
using LinCms.Zero.Domain.Blog;

namespace LinCms.Web.Services.v1.Interfaces
{
    public interface IArticleService
    {
         void Delete(Guid id);

         ArticleDto Get(Guid id);

         void CreateArticle(CreateUpdateArticleDto createArticle);

         void UpdateArticle(CreateUpdateArticleDto updateArticleDto, Article article);
    }
}
