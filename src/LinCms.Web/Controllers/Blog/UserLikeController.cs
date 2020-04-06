using System;
using System.Linq.Expressions;
using AutoMapper;
using DotNetCore.CAP;
using FreeSql;
using LinCms.Application.Contracts.Blog.Notifications;
using LinCms.Application.Contracts.Blog.Notifications.Dtos;
using LinCms.Application.Contracts.Blog.UserLikes;
using LinCms.Application.Contracts.Blog.UserLikes.Dtos;
using LinCms.Core.Data;
using LinCms.Core.Entities.Blog;
using LinCms.Core.Exceptions;
using LinCms.Core.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LinCms.Core.IRepositories;

namespace LinCms.Web.Controllers.Blog
{
    /// <summary>
    ///  用户点赞随笔
    /// </summary>
    [Route("v1/user-like")]
    [ApiController]
    [Authorize]
    public class UserLikeController : ApiControllerBase
    {
        private readonly IAuditBaseRepository<Article> _articleAuditBaseRepository;
        private readonly IAuditBaseRepository<UserLike> _userLikeRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        private readonly IAuditBaseRepository<Comment> _commentRepository;
        private readonly ICapPublisher _capBus;

        public UserLikeController(IMapper mapper,
            ICurrentUser currentUser,
            IAuditBaseRepository<UserLike> userLikeRepository,
            IAuditBaseRepository<Article> articleAuditBaseRepository,
            IAuditBaseRepository<Comment> commentRepository,
            ICapPublisher capBus,
            IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _mapper = mapper;
            _currentUser = currentUser;
            _userLikeRepository = userLikeRepository;
            _articleAuditBaseRepository = articleAuditBaseRepository;
            _commentRepository = commentRepository;
            _capBus = capBus;
        }

        /// <summary>
        /// 用户点赞/取消点赞文章、评论 
        /// </summary>
        /// <param name="createUpdateUserLike"></param>
        /// <returns></returns>
        [HttpPost]
        public UnifyResponseDto Create([FromBody] CreateUpdateUserLikeDto createUpdateUserLike)
        {
            Expression<Func<UserLike, bool>> predicate = r =>
                r.SubjectId == createUpdateUserLike.SubjectId && r.CreateUserId == _currentUser.Id;

            bool exist = _userLikeRepository.Select.Any(predicate);
            int increaseLikeQuantity = 1;

            if (exist)
            {
                increaseLikeQuantity = -1;
                _userLikeRepository.Delete(predicate);
            }

            switch (createUpdateUserLike.SubjectType)
            {
                case UserLikeSubjectType.UserLikeArticle:
                    this.UpdateArticleLike(createUpdateUserLike.SubjectId, increaseLikeQuantity);
                    break;
                case UserLikeSubjectType.UserLikeComment:
                    this.UpdateCommentLike(createUpdateUserLike.SubjectId, increaseLikeQuantity);
                    break;
            }

            if (exist)
            {
                return UnifyResponseDto.Success("取消点赞成功");
            }
  
            UserLike userLike = _mapper.Map<UserLike>(createUpdateUserLike);
            _userLikeRepository.Insert(userLike);

            this.PublishUserLikeNotification(createUpdateUserLike);

            return UnifyResponseDto.Success("点赞成功");
        }

        private void UpdateArticleLike(Guid subjectId, int likesQuantity)
        {
            Article article = _articleAuditBaseRepository.Where(r => r.Id == subjectId).ToOne();
            if (article.IsAudit == false)
            {
                throw new LinCmsException("该文章因违规被拉黑");
            }
            //防止数量一直减，减到小于0
            if (likesQuantity < 0)
            {
                if (article.LikesQuantity < -likesQuantity)
                {
                    return;
                }
            }

            _articleAuditBaseRepository
                .UpdateDiy.Set(r => r.LikesQuantity + likesQuantity).Where(r => r.Id == subjectId).ExecuteAffrows();
        }

        private void UpdateCommentLike(Guid subjectId, int likesQuantity)
        {
            Comment comment = _commentRepository.Select.Where(r => r.Id == subjectId).ToOne();
            if (comment.IsAudit == false)
            {
                throw new LinCmsException("该评论因违规被拉黑");
            }
            //防止数量一直减，减到小于0
            if (likesQuantity < 0)
            {
                if (comment.LikesQuantity < -likesQuantity)
                {
                    return;
                }
            }
            _commentRepository.UpdateDiy.Set(r => r.LikesQuantity + likesQuantity).Where(r => r.Id == subjectId).ExecuteAffrows();
        }

        private void PublishUserLikeNotification(CreateUpdateUserLikeDto createUpdateUserLike)
        {
            //根据用户点赞类型：文章、评论，得到消息的NotificationRespUserId的值
            var createNotificationDto = new CreateNotificationDto()
            {
                UserInfoId = _currentUser.Id ?? 0,
                CreateTime = DateTime.Now,
            };

            switch (createUpdateUserLike.SubjectType)
            {
                case UserLikeSubjectType.UserLikeArticle:

                    Article subjectArticle = _articleAuditBaseRepository.Where(r => r.Id == createUpdateUserLike.SubjectId).ToOne();

                    createNotificationDto.NotificationRespUserId = subjectArticle.CreateUserId;
                    createNotificationDto.NotificationType = NotificationType.UserLikeArticle;
                    createNotificationDto.ArticleId = createUpdateUserLike.SubjectId;
                    break;

                case UserLikeSubjectType.UserLikeComment:

                    Comment subjectComment = _commentRepository.Where(r => r.Id == createUpdateUserLike.SubjectId).ToOne();

                    createNotificationDto.NotificationRespUserId = subjectComment.CreateUserId;
                    createNotificationDto.NotificationType = NotificationType.UserLikeArticleComment;
                    createNotificationDto.ArticleId = subjectComment.SubjectId;
                    createNotificationDto.CommentId = createUpdateUserLike.SubjectId;
                    break;
            }


            if (createNotificationDto.NotificationRespUserId != 0 && _currentUser.Id != createNotificationDto.NotificationRespUserId)
            {
                using ICapTransaction trans = UnitOfWork.BeginTransaction(_capBus, false);

                _capBus.Publish("NotificationController.Post", createNotificationDto);

                trans.Commit();
            }
        }
    }
}