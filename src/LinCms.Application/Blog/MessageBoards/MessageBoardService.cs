using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Application.Contracts.Blog.MessageBoards;
using LinCms.Application.Contracts.Cms.Users;
using LinCms.Core.Common;
using LinCms.Core.Data;
using LinCms.Core.Entities;
using LinCms.Core.Entities.Blog;
using LinCms.Core.Exceptions;
using LinCms.Core.Extensions;
using LinCms.Core.IRepositories;
using LinCms.Core.Security;
using Microsoft.AspNetCore.Http;

namespace LinCms.Application.Blog.MessageBoards
{
    public class MessageBoardService : IMessageBoardService
    {
        private readonly IAuditBaseRepository<MessageBoard> _messageBoardRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        public MessageBoardService(
            IAuditBaseRepository<MessageBoard> messageBoardRepository,
            IMapper mapper, 
            IHttpContextAccessor httpContextAccessor, 
            IUserService userService, 
            ICurrentUser currentUser
            )
        {
            _messageBoardRepository = messageBoardRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _currentUser = currentUser;
        }

        public async Task<PagedResultDto<MessageBoardDto>> GetListAsync(PageDto pageDto)
        {
            List<MessageBoardDto> entitiesBoardDtos =(await _messageBoardRepository
                .Select
                .OrderByDescending(r => r.CreateTime)
                .ToPagerListAsync(pageDto, out long totalCount))
                .Select(r => _mapper.Map<MessageBoardDto>(r)).ToList();

            return new PagedResultDto<MessageBoardDto>(entitiesBoardDtos, totalCount);
        }


        private string GetIp()
        {
            string ip = _httpContextAccessor.HttpContext.Request.Headers["X-Real-IP"].FirstOrDefault();

            if (string.IsNullOrEmpty(ip))
            {

                ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();

            }
            return ip;
        }


        public async Task CreateAsync(CreateMessageBoardDto createMessageBoardDto)
        {
            MessageBoard messageBoard = _mapper.Map<MessageBoard>(createMessageBoardDto);

            messageBoard.Ip = this.GetIp();
            messageBoard.Agent = _httpContextAccessor.HttpContext.Request.Headers["User-agent"].ToString();
            messageBoard.UserHost = Dns.GetHostName();
            messageBoard.System = LinCmsUtils.GetOsNameByUserAgent(messageBoard.Agent);
            if (messageBoard.Ip.IsNotNullOrEmpty())
            {
                IpQueryResult ipQueryResult = LinCmsUtils.IpQueryCity(messageBoard.Ip);
                messageBoard.GeoPosition = ipQueryResult.errno == 0 ? ipQueryResult.data : ipQueryResult.errmsg;
            }

            LinUser linUser = await _userService.GetCurrentUserAsync();
            if (linUser == null)
            {
                messageBoard.Avatar = "/assets/user/" + new Random().Next(1, 360) + ".png";
            }
            else
            {
                messageBoard.Avatar = _currentUser.GetFileUrl(linUser.Avatar);
            }

            await  _messageBoardRepository.InsertAsync(messageBoard);
        }


        public async Task UpdateAsync(Guid id, bool isAudit)
        {
            MessageBoard messageBoard =await _messageBoardRepository.Select.Where(r => r.Id == id).ToOneAsync();
            if (messageBoard == null)
            {
                throw new LinCmsException("没有找到相关留言");
            }

            messageBoard.IsAudit = isAudit;
            await _messageBoardRepository.UpdateAsync(messageBoard);
        }
    }
}
