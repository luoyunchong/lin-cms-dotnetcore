﻿using System;
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
        public PagedResultDto<NotificationDto> Get([FromQuery] NotificationSearchDto pageDto)
        {
            return _notificationService.Get(pageDto);
        }

        [NonAction]
        [CapSubscribe("NotificationController.Post")]
        public async Task<UnifyResponseDto> CreateAsync([FromBody] CreateNotificationDto createNotification)
        {
            await _notificationService.CreateAsync(createNotification);
            return UnifyResponseDto.Success("新建消息成功");
        }

        [HttpPut("{id}")]
        public void SetNotificationRead(Guid id)
        {
            _notificationService.SetNotificationRead(id);
        }
    }
}