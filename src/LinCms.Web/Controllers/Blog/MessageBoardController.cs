using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Application.Cms.Users;
using LinCms.Application.Contracts.Blog.MessageBoards;
using LinCms.Application.Contracts.Cms.Users;
using LinCms.Core.Aop;
using LinCms.Core.Common;
using LinCms.Core.Data;
using LinCms.Core.Entities;
using LinCms.Core.Entities.Blog;
using LinCms.Core.Exceptions;
using LinCms.Core.Extensions;
using LinCms.Core.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LinCms.Core.IRepositories;

namespace LinCms.Web.Controllers.Blog
{
    /// <summary>
    /// 留言板
    /// </summary>
    [Route("v1/message-board")]
    [ApiController]
    public class MessageBoardController : ControllerBase
    {
        private readonly IMessageBoardService _messageBoardService;
        public MessageBoardController(IMessageBoardService messageBoardService)
        {
            _messageBoardService = messageBoardService;
        }

        /// <summary>
        /// 留言板倒序
        /// </summary>
        /// <param name="pageDto"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<PagedResultDto<MessageBoardDto>> GetListAsync([FromQuery]PageDto pageDto)
        {
          return await _messageBoardService.GetListAsync(pageDto);
        }

        /// <summary>
        /// 用户留言，无须登录
        /// </summary>
        /// <param name="createMessageBoardDto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<UnifyResponseDto> CreateAsync([FromBody] CreateMessageBoardDto createMessageBoardDto)
        {
            await _messageBoardService.CreateAsync(createMessageBoardDto);
            return UnifyResponseDto.Success("留言成功");
        }

        /// <summary>
        /// 审核评论-拉黑/取消拉黑
        /// </summary>
        /// <param name="id">审论Id</param>
        /// <param name="isAudit"></param>
        /// <returns></returns>
        [LinCmsAuthorize("审核留言", "留言板")]
        [HttpPut("{id}")]
        public async Task<UnifyResponseDto> UpdateAsync(Guid id, bool isAudit)
        {
            await _messageBoardService.UpdateAsync(id, isAudit);
            return UnifyResponseDto.Success();
        }

    }
}