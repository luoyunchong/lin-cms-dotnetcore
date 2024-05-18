using System;
using System.Threading.Tasks;
using DotNetCore.CAP;
using IGeekFan.FreeKit.Extras.Dto;
using LinCms.Blog.Notifications;
using LinCms.Data;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Controllers.Blog;

/// <summary>
/// 消息通知
/// </summary>
[ApiExplorerSettings(GroupName = "blog")]
[Area("blog")]
[Route("api/blog/notifications")]
[ApiController]
public class NotificationController(INotificationService notificationService) : ControllerBase
{
    [HttpGet]
    public async Task<PagedResultDto<NotificationDto>> GetListAsync([FromQuery] NotificationSearchDto pageDto)
    {
        return await notificationService.GetListAsync(pageDto);
    }

    [NonAction]
    [CapSubscribe(CreateNotificationDto.CreateOrCancelAsync)]
    public async Task<UnifyResponseDto> CreateOrCancelAsync([FromBody] CreateNotificationDto createNotification)
    {
        await notificationService.CreateOrCancelAsync(createNotification);
        return UnifyResponseDto.Success("新建消息成功");
    }

    [HttpPut("{id}")]
    public async Task SetNotificationReadAsync(Guid id)
    {
        await notificationService.SetNotificationReadAsync(id);
    }
}