using AutoMapper;
using LinCms.Web.Models.v1.Articles;
using LinCms.Web.Models.v1.Books;
using LinCms.Zero.Domain;
using LinCms.Zero.Domain.Blog;

namespace LinCms.Web.AutoMapper.v1
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
