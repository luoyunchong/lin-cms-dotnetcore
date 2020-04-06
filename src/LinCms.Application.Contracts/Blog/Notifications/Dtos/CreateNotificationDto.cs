using System;
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.Contracts.Blog.Notifications.Dtos
{
    public class CreateNotificationDto
    {
        public NotificationType NotificationType { get; set; }
        public Guid? ArticleId { get; set; }
        public Guid? CommentId { get; set; }
        public long NotificationRespUserId { get; set; }
        public long UserInfoId { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
