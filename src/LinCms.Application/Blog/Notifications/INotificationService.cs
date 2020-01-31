using System;
using LinCms.Application.Contracts.Blog.Notifications;
using LinCms.Core.Data;

namespace LinCms.Application.Blog.Notifications
{
    public interface INotificationService
    {
        PagedResultDto<NotificationDto> Get(NotificationSearchDto pageDto);
        void Post(CreateNotificationDto createNotificationDto);

        void SetNotificationRead(Guid id);
    }
}
