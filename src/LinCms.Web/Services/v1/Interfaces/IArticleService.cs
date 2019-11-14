using System;
using LinCms.Web.Models.v1.Articles;

namespace LinCms.Web.Services.v1.Interfaces
{
    public interface IArticleService
    {
         void Delete(Guid id);

         ArticleDto Get(Guid id);
    }
}
