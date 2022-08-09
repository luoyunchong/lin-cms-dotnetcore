using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LinCms.Blog.Articles;
using LinCms.Blog.Comments;
using LinCms.Entities.Blog;
using LinCms.IRepositories;

namespace LinCms.Blog.UserLikes
{
    public class UserLikeService : ApplicationService, IUserLikeService
    {
        private readonly IAuditBaseRepository<UserLike> _userLikeRepository;
        private readonly IArticleService _articleService;
        private readonly ICommentService _commentService;

        public UserLikeService(
            IAuditBaseRepository<UserLike> userLikeRepository,
            IArticleService articleService,
            ICommentService commentService)
        {
            _userLikeRepository = userLikeRepository;
            _articleService = articleService;
            _commentService = commentService;
        }

        public async Task<bool> CreateOrCancelAsync(CreateUpdateUserLikeDto createUpdateUserLike)
        {
            Expression<Func<UserLike, bool>> predicate = r => r.SubjectId == createUpdateUserLike.SubjectId && r.CreateUserId == CurrentUser.Id;

            bool exist = await _userLikeRepository.Select.AnyAsync(predicate);
            int increaseLikeQuantity = 1;

            if (exist)
            {
                increaseLikeQuantity = -1;
                await _userLikeRepository.DeleteAsync(predicate);
            }

            switch (createUpdateUserLike.SubjectType)
            {
                case UserLikeSubjectType.UserLikeArticle:
                    await _articleService.UpdateLikeQuantityAysnc(createUpdateUserLike.SubjectId, increaseLikeQuantity);
                    break;
                case UserLikeSubjectType.UserLikeComment:
                    await _commentService.UpdateLikeQuantityAysnc(createUpdateUserLike.SubjectId, increaseLikeQuantity);
                    break;
                default:
                    break;
            }

            if (exist)
            {
                return true;
            }

            UserLike userLike = Mapper.Map<UserLike>(createUpdateUserLike);
            await _userLikeRepository.InsertAsync(userLike);
            return false;
        }
    }
}
