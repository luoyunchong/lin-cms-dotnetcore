using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LinCms.Application.Contracts.Blog.ArticleDrafts.Dtos;
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.Contracts.Blog.ArticleDrafts
{
    public interface IArticleDraftService
    {
        Task UpdateAsync(Guid id, UpdateArticleDraftDto updateArticleDto);

        Task<ArticleDraftDto> GetAsync(Guid id);
    }
}
