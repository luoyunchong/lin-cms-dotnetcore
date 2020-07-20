using AutoMapper;
using LinCms.Blog.ArticleDrafts;
using LinCms.Entities.Blog;

namespace LinCms.Blog.Articles
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
