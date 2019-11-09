using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AutoMapper;
using LinCms.Web.Models.v1.Comments;
using LinCms.Web.Services.Interfaces;
using LinCms.Zero.Aop;
using LinCms.Zero.Common;
using LinCms.Zero.Data;
using LinCms.Zero.Domain;
using LinCms.Zero.Domain.Blog;
using LinCms.Zero.Exceptions;
using LinCms.Zero.Extensions;
using LinCms.Zero.Repositories;
using LinCms.Zero.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.v1
{
    /// <summary>
    /// 随笔评论
    /// </summary>
    [Route("v1/comment")]
    [ApiController]
    [Authorize]
    public class CommentController : ControllerBase
    {

        private readonly AuditBaseRepository<Comment> _commentAuditBaseRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUser _currentUser;
        private readonly IUserSevice _userService;

        public CommentController(AuditBaseRepository<Comment> commentAuditBaseRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, ICurrentUser currentUser, IUserSevice userService)
        {
            _commentAuditBaseRepository = commentAuditBaseRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _currentUser = currentUser;
            _userService = userService;
        }

        /// <summary>
        /// 评论分页列表页,当随笔Id有值时，根据随笔查询所在文章
        /// </summary>
        /// <param name="commentSearchDto"></param>
        /// <returns></returns>
        [HttpGet]
        public PagedResultDto<CommentDto> Get([FromQuery]CommentSearchDto commentSearchDto)
        {
            List<CommentDto> comments = _commentAuditBaseRepository
                .Select
                .WhereIf(commentSearchDto.ArticleId.HasValue,r=>r.ArticleId==commentSearchDto.ArticleId)
                .OrderByDescending(r => r.Id)
                .ToPagerList(commentSearchDto, out long totalCount)
                .Select(r => _mapper.Map<CommentDto>(r)).ToList();

            return new PagedResultDto<CommentDto>(comments, totalCount);
        }

        [HttpDelete("{id}")]
        [LinCmsAuthorize("删除评论", "评论")]
        public ResultDto Delete(Guid id)
        {
            _commentAuditBaseRepository.Delete(new Comment { Id = id });
            return ResultDto.Success();
        }


        /// <summary>
        /// 用户留言，可登录，已登录用户自动获取头像
        /// </summary>
        /// <param name="createCommentDto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public ResultDto Post([FromBody] CreateCommentDto createCommentDto)
        {
            Comment comment = _mapper.Map<Comment>(createCommentDto);

            comment.Ip = this.GetIp();
            comment.Agent = Request.Headers["User-agent"].ToString();
            comment.UserHost = Dns.GetHostName();
            comment.System = LinCmsUtils.GetOsNameByUserAgent(comment.Agent);
            if (comment.Ip.IsNotNullOrEmpty())
            {
                IpQueryResult ipQueryResult = LinCmsUtils.IpQueryCity(comment.Ip);
                comment.GeoPosition = ipQueryResult.errno == 0 ? ipQueryResult.data : ipQueryResult.errmsg;
            }

            LinUser linUser = _userService.GetCurrentUser();
            if (linUser == null)
            {
                comment.Avatar = "/assets/user/" + new Random().Next(1, 360) + ".png";
            }
            else
            {
                comment.Avatar = _currentUser.GetFileUrl(linUser.Avatar);
            }

            _commentAuditBaseRepository.Insert(comment);
            return ResultDto.Success("留言成功");
        }

        /// <summary>
        /// 审核评论-拉黑/取消拉黑
        /// </summary>
        /// <param name="id">审论Id</param>
        /// <param name="isAudit"></param>
        /// <returns></returns>
        [LinCmsAuthorize("审核评论","评论")]
        [HttpPut("{id}")]
        public ResultDto Put(Guid id,bool isAudit)
        {
            Comment comment = _commentAuditBaseRepository.Select.Where(r => r.Id == id).ToOne();
            if (comment == null)
            {
                throw new LinCmsException("没有找到相关评论");
            }

            comment.IsAudited = isAudit;
            _commentAuditBaseRepository.Update(comment);
            return ResultDto.Success();
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
    }
}