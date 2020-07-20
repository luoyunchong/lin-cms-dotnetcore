using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Data;
using LinCms.Entities.Blog;
using LinCms.Extensions;
using LinCms.IRepositories;
using LinCms.Security;

namespace LinCms.Blog.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly IAuditBaseRepository<Notification> _notificationRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        private readonly IFileRepository _fileRepository;

        public NotificationService(IAuditBaseRepository<Notification> notificationRepository, IMapper mapper,
            ICurrentUser currentUser, IFileRepository fileRepository)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
            _currentUser = currentUser;
            _fileRepository = fileRepository;
        }

        public async Task<PagedResultDto<NotificationDto>> GetListAsync(NotificationSearchDto pageDto)
        {
            List<NotificationDto> notification = (await _notificationRepository.Select
                    .Include(r => r.ArticleEntry)
                    .Include(r => r.CommentEntry)
                    .Include(r => r.UserInfo)
                    .WhereIf(pageDto.NotificationSearchType == NotificationSearchType.UserComment,
                        r => r.NotificationType == NotificationType.UserCommentOnArticle ||
                             r.NotificationType == NotificationType.UserCommentOnComment)
                    .WhereIf(pageDto.NotificationSearchType == NotificationSearchType.UserLike,
                        r => r.NotificationType == NotificationType.UserLikeArticle ||
                             r.NotificationType == NotificationType.UserLikeArticleComment)
                    .WhereIf(pageDto.NotificationSearchType == NotificationSearchType.UserLikeUser,
                        r => r.NotificationType == NotificationType.UserLikeUser)
                    .Where(r => r.NotificationRespUserId == _currentUser.Id)
                    .OrderByDescending(r => r.CreateTime)
                    .ToPagerListAsync(pageDto, out long totalCount))
                .Select(r =>
                {
                    NotificationDto notificationDto = _mapper.Map<NotificationDto>(r);
                    if (notificationDto.UserInfo != null)
                    {
                        notificationDto.UserInfo.Avatar = _fileRepository.GetFileUrl(notificationDto.UserInfo.Avatar);
                    }

                    return notificationDto;
                }).ToList();

            return new PagedResultDto<NotificationDto>(notification, totalCount);
        }

        public async Task CreateOrCancelAsync(CreateNotificationDto notificationDto)
        {
            if (notificationDto.IsCancel == false)
            {
                Notification linNotification = _mapper.Map<Notification>(notificationDto);
                await _notificationRepository.InsertAsync(linNotification);
            }
            else
            {
                Expression<Func<Notification, bool>> exprssion = r =>
                    r.NotificationType == notificationDto.NotificationType &&
                    r.UserInfoId == notificationDto.UserInfoId;
                switch (notificationDto.NotificationType)
                {
                    case NotificationType.UserLikeArticle://点赞随笔
                        exprssion = exprssion.And(r => r.ArticleId == notificationDto.ArticleId);
                        break;
                    case NotificationType.UserCommentOnArticle: //评论随笔
                    case NotificationType.UserLikeArticleComment: //点赞随笔下的评论
                        exprssion = exprssion.And(r =>
                            r.ArticleId == notificationDto.ArticleId && r.CommentId == notificationDto.CommentId);
                        break;
                    case NotificationType.UserCommentOnComment: //回复评论
                        exprssion = exprssion.And(r =>
                            r.ArticleId == notificationDto.ArticleId && r.CommentId == notificationDto.CommentId &&
                            r.NotificationRespUserId == notificationDto.NotificationRespUserId);
                        break;
                    case NotificationType.UserLikeUser: //关注用户
                        exprssion = exprssion.And(r =>
                            r.NotificationRespUserId == notificationDto.NotificationRespUserId);
                        break;
                    default:
                        exprssion = r => false;
                        break;
                }

                await _notificationRepository.DeleteAsync(exprssion);
            }
        }

        public async Task SetNotificationReadAsync(Guid id)
        {
            await _notificationRepository.UpdateDiy.Where(r => r.Id == id).Set(r => r.IsRead, true)
                .ExecuteAffrowsAsync();
        }
    }
}