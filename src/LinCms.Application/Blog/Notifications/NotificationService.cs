﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LinCms.Application.Contracts.Blog.Notifications;
using LinCms.Core.Data;
using LinCms.Core.Entities.Blog;
using LinCms.Core.Extensions;
using LinCms.Core.Security;
using LinCms.Infrastructure.Repositories;

namespace LinCms.Application.Blog.Notifications
{
    public class NotificationService : INotificationService
    {
        private readonly AuditBaseRepository<Notification> _notificationRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        public NotificationService(AuditBaseRepository<Notification> notificationRepository, IMapper mapper, ICurrentUser currentUser)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public PagedResultDto<NotificationDto> Get(NotificationSearchDto pageDto)
        {
            List<NotificationDto> notification = _notificationRepository.Select
                .Include(r => r.ArticleEntry)
                .Include(r => r.CommentEntry)
                .Include(r => r.UserInfo)

                .WhereIf(pageDto.NotificationSearchType == NotificationSearchType.UserComment, 
                    r => r.NotificationType == NotificationType.UserCommentOnArticle||r.NotificationType==NotificationType.UserCommentOnComment)

                .WhereIf(pageDto.NotificationSearchType == NotificationSearchType.UserLike,
                    r => r.NotificationType == NotificationType.UserLikeArticle || r.NotificationType == NotificationType.UserLikeArticleComment)

                .WhereIf(pageDto.NotificationSearchType == NotificationSearchType.UserLikeUser,
                    r => r.NotificationType == NotificationType.UserLikeUser)

                .Where(r => r.NotificationRespUserId == _currentUser.Id)
                .OrderByDescending(r => r.CreateTime)
                .ToPagerList(pageDto, out long totalCount)
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

        public void Post(CreateNotificationDto createNotificationDto)
        {
            Notification linNotification = _mapper.Map<Notification>(createNotificationDto);
            _notificationRepository.Insert(linNotification);
            _notificationRepository.UnitOfWork.Commit();
        }

        public void SetNotificationRead(Guid id)
        {
            _notificationRepository.UpdateDiy.Where(r => r.Id == id).Set(r => r.IsRead, true);
        }
    }
}
