using System;
using System.Threading.Tasks;

namespace LinCms.Blog.ArticleDrafts;

public interface IArticleDraftService
{
    Task UpdateAsync(Guid id, UpdateArticleDraftDto updateArticleDto);

    Task<ArticleDraftDto> GetAsync(Guid id);
}