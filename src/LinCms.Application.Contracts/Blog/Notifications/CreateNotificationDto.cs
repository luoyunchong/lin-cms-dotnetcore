using System;
using LinCms.Entities.Blog;

namespace LinCms.Blog.Notifications
{
    public class CreateNotificationDto
    {
        public NotificationType NotificationType { get; set; }
        public Guid? ArticleId { get; set; }
        public Guid? CommentId { get; set; }
        public long NotificationRespUserId { get; set; }
        public long UserInfoId { get; set; }
        
        public bool IsCancel { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
