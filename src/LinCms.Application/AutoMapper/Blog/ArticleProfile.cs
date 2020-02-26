using AutoMapper;
using LinCms.Application.Contracts.Blog.Articles;
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.AutoMapper.Blog
{
    public class ArticleProfile : Profile
    {
        public ArticleProfile()
        {
            CreateMap<CreateUpdateArticleDto, Article>();
            CreateMap<Article, ArticleDto>();
            CreateMap<Article, ArticleListDto>();
        }
    }
}
