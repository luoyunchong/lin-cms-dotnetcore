using System;
using System.Threading.Tasks;
using DotNetCore.CAP;
using LinCms.Application.Blog.Notifications;
using LinCms.Application.Contracts.Blog.Notifications;
using LinCms.Application.Contracts.Blog.Notifications.Dtos;
using LinCms.Core.Data;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.Blog
{
    [Route("v1/notification")]
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
        [CapSubscribe("NotificationController.Post")]
        public async Task<UnifyResponseDto> CreateAsync([FromBody] CreateNotificationDto createNotification)
        {
            await _notificationService.CreateAsync(createNotification);
            return UnifyResponseDto.Success("新建消息成功");
        }

        [HttpPut("{id}")]
        public async Task SetNotificationReadAsync(Guid id)
        {
            await _notificationService.SetNotificationReadAsync(id);
        }
    }
}