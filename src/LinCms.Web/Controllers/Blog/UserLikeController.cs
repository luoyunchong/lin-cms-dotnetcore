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
using LinCms.Application.Blog.UserSubscribes;
using System.Threading.Tasks;

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
        private readonly IAuditBaseRepository<Article> _articleRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IAuditBaseRepository<Comment> _commentRepository;
        private readonly ICapPublisher _capBus;
        private readonly IUserLikeService _userLikeService;

        public UserLikeController(
            ICurrentUser currentUser,
            IAuditBaseRepository<Article> articleRepository,
            IAuditBaseRepository<Comment> commentRepository,
            ICapPublisher capBus,
            IUnitOfWork unitOfWork, IUserLikeService userLikeService) : base(unitOfWork)
        {
            _currentUser = currentUser;
            _articleRepository = articleRepository;
            _commentRepository = commentRepository;
            _capBus = capBus;
            _userLikeService = userLikeService;
        }

        /// <summary>
        /// 用户点赞/取消点赞文章、评论 
        /// </summary>
        /// <param name="createUpdateUserLike"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<UnifyResponseDto> CreateOrCancelAsync([FromBody] CreateUpdateUserLikeDto createUpdateUserLike)
        {
            string message = await _userLikeService.CreateOrCancelAsync(createUpdateUserLike);

            await this.PublishUserLikeNotification(createUpdateUserLike);

            return UnifyResponseDto.Success(message);
        }

        /// <summary>
        /// 根据用户点赞类型：文章、评论，得到消息的NotificationRespUserId的值
        /// </summary>
        /// <param name="createUpdateUserLike"></param>
        /// <returns></returns>
        private async Task PublishUserLikeNotification(CreateUpdateUserLikeDto createUpdateUserLike)
        {
            var createNotificationDto = new CreateNotificationDto()
            {
                UserInfoId = _currentUser.Id ?? 0,
                CreateTime = DateTime.Now,
            };

            switch (createUpdateUserLike.SubjectType)
            {
                case UserLikeSubjectType.UserLikeArticle:

                    Article subjectArticle = await _articleRepository.Where(r => r.Id == createUpdateUserLike.SubjectId).ToOneAsync();

                    createNotificationDto.NotificationRespUserId = subjectArticle.CreateUserId;
                    createNotificationDto.NotificationType = NotificationType.UserLikeArticle;
                    createNotificationDto.ArticleId = createUpdateUserLike.SubjectId;
                    break;

                case UserLikeSubjectType.UserLikeComment:

                    Comment subjectComment = await _commentRepository.Where(r => r.Id == createUpdateUserLike.SubjectId).ToOneAsync();

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