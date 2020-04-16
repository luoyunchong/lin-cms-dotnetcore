using System;
using System.Threading.Tasks;
using LinCms.Application.Contracts.Blog.Notifications.Dtos;
using LinCms.Core.Data;

namespace LinCms.Application.Contracts.Blog.Notifications
{
    public interface INotificationService
    {
        PagedResultDto<NotificationDto> Get(NotificationSearchDto pageDto);

        /// <summary>
        /// 新增一个消息通知
        /// </summary>
        /// <param name="createNotificationDto"></param>
        Task CreateAsync(CreateNotificationDto createNotificationDto);

        /// <summary>
        /// 设置消息通知为已读状态
        /// </summary>
        /// <param name="id"></param>
        void SetNotificationRead(Guid id);
    }
}
