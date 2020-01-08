using AutoMapper;
using LinCms.Application.Contracts.v1.Articles;
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.AutoMapper.v1
{
    public class ArticleProfile : Profile
    {
        public ArticleProfile()
        {
            CreateMap<CreateUpdateArticleDto, Article>();
            CreateMap<Article, ArticleDto>();
        }
    }
}
