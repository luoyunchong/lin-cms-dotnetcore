using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Application.Contracts.Blog.Articles;
using LinCms.Application.Contracts.Blog.Comments;
using LinCms.Application.Contracts.Blog.UserLikes.Dtos;
using LinCms.Core.Entities.Blog;
using LinCms.Core.IRepositories;
using LinCms.Core.Security;

namespace LinCms.Application.Blog.UserSubscribes
{
    public class UserLikeService : IUserLikeService
    {
        private readonly IAuditBaseRepository<UserLike> _userLikeRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        private readonly IArticleService _articleService;
        private readonly ICommentService _commentService;

        public UserLikeService(IMapper mapper,
            ICurrentUser currentUser,
            IAuditBaseRepository<UserLike> userLikeRepository,
            IArticleService articleService,
            ICommentService commentService)
        {
            _mapper = mapper;
            _currentUser = currentUser;
            _userLikeRepository = userLikeRepository;
            _articleService = articleService;
            _commentService = commentService;
        }

        public async Task<string> CreateOrCancelAsync(CreateUpdateUserLikeDto createUpdateUserLike)
        {
            Expression<Func<UserLike, bool>> predicate = r =>
                r.SubjectId == createUpdateUserLike.SubjectId && r.CreateUserId == _currentUser.Id;

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
                    await _articleService.UpdateLikeQuantity(createUpdateUserLike.SubjectId, increaseLikeQuantity);
                    break;
                case UserLikeSubjectType.UserLikeComment:
                    await _commentService.UpdateLikeQuantity(createUpdateUserLike.SubjectId, increaseLikeQuantity);
                    break;
            }

            if (exist)
            {
                return "取消点赞成功";
            }


            UserLike userLike = _mapper.Map<UserLike>(createUpdateUserLike);
            _userLikeRepository.Insert(userLike);

            return "点赞成功";
        }
    }
}
