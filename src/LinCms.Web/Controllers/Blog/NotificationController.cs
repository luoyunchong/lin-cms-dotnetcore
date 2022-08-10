using System;
using System.Threading.Tasks;
using DotNetCore.CAP;
using LinCms.Blog.Notifications;
using LinCms.Data;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Controllers.Blog
{
    [ApiExplorerSettings(GroupName = "blog")]
    [Area("blog")]
    [Route("api/blog/notifications")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<PagedResultDto<NotificationDto>> GetListAsync([FromQuery] NotificationSearchDto pageDto)
        {
            return await _notificationService.GetListAsync(pageDto);
        }

        [NonAction]
        [CapSubscribe(CreateNotificationDto.CreateOrCancelAsync)]
        public async Task<UnifyResponseDto> CreateOrCancelAsync([FromBody] CreateNotificationDto createNotification)
        {
            await _notificationService.CreateOrCancelAsync(createNotification);
            return UnifyResponseDto.Success("新建消息成功");
        }

        [HttpPut("{id}")]
        public async Task SetNotificationReadAsync(Guid id)
        {
            await _notificationService.SetNotificationReadAsync(id);
        }
    }
}