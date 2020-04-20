using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Application.Contracts.Blog.Notifications;
using LinCms.Application.Contracts.Blog.Notifications.Dtos;
using LinCms.Core.Data;
using LinCms.Core.Entities.Blog;
using LinCms.Core.Extensions;
using LinCms.Core.IRepositories;
using LinCms.Core.Security;

namespace LinCms.Application.Blog.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly IAuditBaseRepository<Notification> _notificationRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public NotificationService(IAuditBaseRepository<Notification> notificationRepository, IMapper mapper,
            ICurrentUser currentUser)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
            _currentUser = currentUser;
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
                        notificationDto.UserInfo.Avatar = _currentUser.GetFileUrl(notificationDto.UserInfo.Avatar);
                    }

                    return notificationDto;
                }).ToList();

            return new PagedResultDto<NotificationDto>(notification, totalCount);
        }

        public async Task CreateAsync(CreateNotificationDto createNotificationDto)
        {
            Notification linNotification = _mapper.Map<Notification>(createNotificationDto);
            await _notificationRepository.InsertAsync(linNotification);
            _notificationRepository.UnitOfWork.Commit();
        }

        public async Task SetNotificationReadAsync(Guid id)
        {
           await _notificationRepository.UpdateDiy.Where(r => r.Id == id).Set(r => r.IsRead, true).ExecuteAffrowsAsync();
        }
    }
}