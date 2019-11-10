using System;
using AutoMapper;
using FreeSql;
using LinCms.Web.Models.v1.Articles;
using LinCms.Web.Services.Interfaces;
using LinCms.Zero.Domain.Blog;
using LinCms.Zero.Repositories;
using LinCms.Zero.Security;

namespace LinCms.Web.Services
{
    public class ArticleAppService:IArticleService
    {
        private readonly AuditBaseRepository<Article> _articleRepository;
        private readonly GuidRepository<TagArticle> _tagArticleRepository;
        private readonly AuditBaseRepository<UserLike> _userLikeRepository;
        private readonly AuditBaseRepository<Comment> _commentBaseRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public ArticleAppService(AuditBaseRepository<Article> articleRepository, GuidRepository<TagArticle> tagArticleRepository, IMapper mapper, ICurrentUser currentUser, AuditBaseRepository<UserLike> userLikeRepository, AuditBaseRepository<Comment> commentBaseRepository)
        {
            _articleRepository = articleRepository;
            _tagArticleRepository = tagArticleRepository;
            _mapper = mapper;
            _currentUser = currentUser;
            _userLikeRepository = userLikeRepository;
            _commentBaseRepository = commentBaseRepository;
        }

        public void Delete(Guid id)
        {
            _tagArticleRepository.Delete(r => r.ArticleId == id);
            _articleRepository.Delete(new Article { Id = id });
        }

        public ArticleDto Get(Guid id)
        {
            Article article = _articleRepository.Select.IncludeMany(r => r.Tags).Where(a => a.Id == id).ToOne();

            ArticleDto articleDto = _mapper.Map<ArticleDto>(article);

            articleDto.IsLiked = _userLikeRepository.Select.Any(r => r.SubjectId == id && r.CreateUserId == _currentUser.Id);

            articleDto.IsComment = _commentBaseRepository.Select.Any(r => r.SubjectId == id&&r.CreateUserId==_currentUser.Id);

            articleDto.ThumbnailDisplay = _currentUser.GetFileUrl(article.Thumbnail);

            return articleDto;
        }
    }
}
