using System;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dto;

namespace LinCms.Blog.Notifications;

/// <summary>
/// 消息通知
/// </summary>
public interface INotificationService
{
    Task<PagedResultDto<NotificationDto>> GetListAsync(NotificationSearchDto pageDto);

    /// <summary>
    /// 新增一个消息通知,或取消消息通知
    /// </summary>
    /// <param name="createNotificationDto"></param>
    Task CreateOrCancelAsync(CreateNotificationDto createNotificationDto);

    /// <summary>
    /// 设置消息通知为已读状态
    /// </summary>
    /// <param name="id"></param>
    Task SetNotificationReadAsync(Guid id);
}