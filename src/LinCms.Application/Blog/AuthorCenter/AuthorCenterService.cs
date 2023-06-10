using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Entities.Blog;
using LinCms.Security;
using System.Threading.Tasks;

namespace LinCms.Blog.AuthorCenter
{
    /// <summary>
    /// 创建者中心统计
    /// </summary>
    public class AuthorCenterService : ApplicationService, IAuthorCenterService
    {
        private readonly IAuditBaseRepository<Article> _articleRepository;
        private readonly IAuditBaseRepository<UserLike> _userLikeRepository;
        private readonly IAuditBaseRepository<Comment> _commentRepository;

        public AuthorCenterService(IAuditBaseRepository<Article> articleRepository,
            IAuditBaseRepository<UserLike> userLikeRepository, IAuditBaseRepository<Comment> commentRepository)
        {
            _articleRepository = articleRepository;
            _userLikeRepository = userLikeRepository;
            _commentRepository = commentRepository;
        }

        public async Task<ArticleCardDto> GetArtcileCardAsync()
        {
            long? userid = CurrentUser.FindUserId();
            var allArticle = await _articleRepository.Select.Where(r => r.CreateUserId == userid.Value).CountAsync();
            var allArticleView = (long) await _articleRepository.Select.Where(r => r.CreateUserId == userid.Value)
                .SumAsync(r => r.ViewHits);
            var allArticleStar = await _userLikeRepository.Select.Where(r =>
                r.CreateUserId == userid.Value && r.SubjectType == UserLikeSubjectType.UserLikeArticle).CountAsync();
            var allArticleComment = (long) await _articleRepository.Select.Where(r => r.CreateUserId == userid.Value)
                .SumAsync(r => r.CommentQuantity);
            var allArticleCollect = (long) await _articleRepository.Select.Where(r => r.CreateUserId == userid.Value)
                .SumAsync(r => r.CollectQuantity);

            return new ArticleCardDto()
            {
                AllArticle = allArticle,
                AllArticleView = allArticleView,
                AllArticleStar = allArticleStar,
                AllArticleComment = allArticleComment,
                AllArticleCollect = allArticleCollect
            };
        }
    }
}