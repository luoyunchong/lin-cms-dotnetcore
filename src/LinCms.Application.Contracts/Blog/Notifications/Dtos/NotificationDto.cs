using System;
using LinCms.Application.Contracts.Cms.Users.Dtos;
using LinCms.Core.Entities;
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.Contracts.Blog.Notifications.Dtos
{
    public class NotificationDto : EntityDto<Guid>
    {
        public NotificationType NotificationType { get; set; }
        public OpenUserDto UserInfo { get; set; }
        public bool IsRead { get; set; }
        public long UserInfoId { get; set; }
        public long MessageRespUserId { get; set; }
        public DateTime CreateTime { get; set; }
        public ArticleEntry ArticleEntry { get; set; }
        public CommentEntry CommentEntry { get; set; }
    }
}
