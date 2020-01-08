using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AutoMapper;
using LinCms.Application.Cms.Users;
using LinCms.Core.Aop;
using LinCms.Core.Common;
using LinCms.Core.Data;
using LinCms.Core.Entities;
using LinCms.Core.Entities.Blog;
using LinCms.Core.Exceptions;
using LinCms.Infrastructure.Repositories;
using LinCms.Core.Security;
using LinCms.Application.Contracts.v1.MessageBoards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LinCms.Core.Extensions;

namespace LinCms.Web.Controllers.v1
{
    /// <summary>
    /// 留言板
    /// </summary>
    [Route("v1/message-board")]
    [ApiController]
    public class MessageBoardController : ControllerBase
    {
        private readonly AuditBaseRepository<MessageBoard> _messageBoardRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserSevice _userService;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        public MessageBoardController(AuditBaseRepository<MessageBoard> messageBoardRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUserSevice userService, ICurrentUser currentUser)
        {
            _messageBoardRepository = messageBoardRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _currentUser = currentUser;
        }

        /// <summary>
        /// 留言板倒序
        /// </summary>
        /// <param name="pageDto"></param>
        /// <returns></returns>
        [HttpGet]
        public PagedResultDto<MessageBoardDto> Get([FromQuery]PageDto pageDto)
        {
            List<MessageBoardDto> entitiesBoardDtos = _messageBoardRepository
                .Select
                .OrderByDescending(r=>r.CreateTime)
                .ToPagerList(pageDto, out long totalCount)
                .Select(r => _mapper.Map<MessageBoardDto>(r)).ToList();

            return new PagedResultDto<MessageBoardDto>(entitiesBoardDtos, totalCount);
        }


        private string GetIp()
        {
            string ip = HttpContext.Request.Headers["X-Real-IP"].FirstOrDefault();

            if (string.IsNullOrEmpty(ip))
            {

                ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

            }
            return ip;
        }

        /// <summary>
        /// 用户留言，无须登录
        /// </summary>
        /// <param name="createMessageBoardDto"></param>
        /// <returns></returns>

        [AllowAnonymous]
        [HttpPost]
        public ResultDto Post([FromBody] CreateMessageBoardDto createMessageBoardDto)
        {
            MessageBoard messageBoard = _mapper.Map<MessageBoard>(createMessageBoardDto);

            messageBoard.Ip = this.GetIp();
            messageBoard.Agent = Request.Headers["User-agent"].ToString();
            messageBoard.UserHost = Dns.GetHostName();
            messageBoard.System = LinCmsUtils.GetOsNameByUserAgent(messageBoard.Agent);
            if (messageBoard.Ip.IsNotNullOrEmpty())
            {
                IpQueryResult ipQueryResult = LinCmsUtils.IpQueryCity(messageBoard.Ip);
                messageBoard.GeoPosition = ipQueryResult.errno == 0 ? ipQueryResult.data : ipQueryResult.errmsg;
            }

            LinUser linUser = _userService.GetCurrentUser();
            if (linUser == null)
            {
                messageBoard.Avatar = "/assets/user/" + new Random().Next(1, 360) + ".png";
            }
            else
            {
                messageBoard.Avatar = _currentUser.GetFileUrl(linUser.Avatar);
            }

            _messageBoardRepository.Insert(messageBoard);
            return ResultDto.Success("留言成功");
        }

        /// <summary>
        /// 审核评论-拉黑/取消拉黑
        /// </summary>
        /// <param name="id">审论Id</param>
        /// <param name="isAudit"></param>
        /// <returns></returns>
        [LinCmsAuthorize("审核留言", "留言板")]
        [HttpPut("{id}")]
        public ResultDto Put(Guid id, bool isAudit)
        {
            MessageBoard messageBoard = _messageBoardRepository.Select.Where(r => r.Id == id).ToOne();
            if (messageBoard == null)
            {
                throw new LinCmsException("没有找到相关留言");
            }

            messageBoard.IsAudit = isAudit;
            _messageBoardRepository.Update(messageBoard);
            return ResultDto.Success();
        }

    }
}