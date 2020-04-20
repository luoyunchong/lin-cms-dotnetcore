using AutoMapper;
using LinCms.Application.Contracts.Blog.ArticleDrafts.Dtos;
using LinCms.Application.Contracts.Blog.Articles.Dtos;
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.Blog.Articles
{
    public class ArticleProfile : Profile
    {
        public ArticleProfile()
        {
            CreateMap<CreateUpdateArticleDto, Article>();
            CreateMap<Article, ArticleDto>();
            CreateMap<Article, ArticleListDto>();
            
            
            CreateMap<UpdateArticleDraftDto,ArticleDraft>();
            
            
            CreateMap<ArticleDraft,ArticleDraftDto>();
            CreateMap<Article,ArticleDraft>();
        }
    }
}
