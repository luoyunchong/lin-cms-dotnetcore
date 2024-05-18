using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dto;
using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Entities.Blog;
using LinCms.Extensions;
using LinCms.IRepositories;
using LinCms.Security;

namespace LinCms.Blog.Notifications;

public class NotificationService(IAuditBaseRepository<Notification> notificationRepository,
        IFileRepository fileRepository)
    : ApplicationService, INotificationService
{
    public async Task<PagedResultDto<NotificationDto>> GetListAsync(NotificationSearchDto pageDto)
    {
        List<NotificationDto> notification = (await notificationRepository.Select
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
                .Where(r => r.NotificationRespUserId ==  CurrentUser.FindUserId())
                .OrderByDescending(r => r.CreateTime)
                .ToPagerListAsync(pageDto, out long totalCount))
            .Select(r =>
            {
                NotificationDto notificationDto = Mapper.Map<NotificationDto>(r);
                if (notificationDto.UserInfo != null)
                {
                    notificationDto.UserInfo.Avatar = fileRepository.GetFileUrl(notificationDto.UserInfo.Avatar);
                }

                return notificationDto;
            }).ToList();

        return new PagedResultDto<NotificationDto>(notification, totalCount);
    }

    public async Task CreateOrCancelAsync(CreateNotificationDto notificationDto)
    {
        if (notificationDto.IsCancel == false)
        {
            Notification linNotification = Mapper.Map<Notification>(notificationDto);
            await notificationRepository.InsertAsync(linNotification);
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

            await notificationRepository.DeleteAsync(exprssion);
        }
    }

    public async Task SetNotificationReadAsync(Guid id)
    {
        await notificationRepository.UpdateDiy.Where(r => r.Id == id).Set(r => r.IsRead, true).ExecuteAffrowsAsync();
    }
}