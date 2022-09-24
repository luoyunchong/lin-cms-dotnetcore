using AutoMapper;
using LinCms.Entities.Blog;

namespace LinCms.Blog.Notifications;

public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<Notification, NotificationDto>();
        CreateMap<Article, ArticleEntry>();
        CreateMap<Comment, CommentEntry>();
        CreateMap<CreateNotificationDto, Notification>();
    }
}