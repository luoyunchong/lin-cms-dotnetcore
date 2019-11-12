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
        private readonly ICurrentUser _currentUser;
        private readonly IUserSevice _userService;

        public CommentController(AuditBaseRepository<Comment> commentAuditBaseRepository, IMapper mapper, ICurrentUser currentUser, IUserSevice userService)
        {
            _commentAuditBaseRepository = commentAuditBaseRepository;
            _mapper = mapper;
            _currentUser = currentUser;
            _userService = userService;
        }

        /// <summary>
        /// 评论分页列表页,当随笔Id有值时，查询出此随笔的评论
        /// </summary>
        /// <param name="commentSearchDto"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public PagedResultDto<CommentDto> Get([FromQuery]CommentSearchDto commentSearchDto)
        {
            long? userId = _currentUser.Id;
            List<CommentDto> comments = _commentAuditBaseRepository
                .Select
                .Include(r => r.UserInfo)
                .Include(r => r.RespUserInfo)
                .IncludeMany(r => r.Childs.Take(2), t => t.Include(u => u.UserInfo).IncludeMany(u => u.UserLikes))
                .IncludeMany(r => r.UserLikes)
                .WhereIf(commentSearchDto.ArticleId.HasValue, r => r.SubjectId == commentSearchDto.ArticleId)
                .Where(r => r.RootCommentId == commentSearchDto.RootCommentId)
                .OrderByDescending(!commentSearchDto.RootCommentId.HasValue, r => r.CreateTime)
                .OrderBy(commentSearchDto.RootCommentId.HasValue, r => r.CreateTime)
                .ToPagerList(commentSearchDto, out long totalCount)
                .Select(r =>
                {
                    CommentDto commentDto = _mapper.Map<CommentDto>(r);


                    commentDto.UserInfo.Avatar = _currentUser.GetFileUrl(commentDto.UserInfo.Avatar);
                    commentDto.TopComment = r.Childs.ToList().Select(u =>
                    {
                        CommentDto childrenDto = _mapper.Map<CommentDto>(u);
                        childrenDto.UserInfo.Avatar = _currentUser.GetFileUrl(childrenDto.UserInfo.Avatar);
                        //childrenDto.IsLiked = userId != null && u.UserLikes.Where(z => z.CreateUserId == userId).IsNotEmpty();
                        return childrenDto;
                    }).ToList();
                    commentDto.IsLiked = userId != null && r.UserLikes.Where(u => u.CreateUserId == userId).IsNotEmpty();
                    return commentDto;
                }).ToList();

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
        [HttpPost]
        public ResultDto Post([FromBody] CreateCommentDto createCommentDto)
        {
            Comment comment = _mapper.Map<Comment>(createCommentDto);
            _commentAuditBaseRepository.Insert(comment);
            return ResultDto.Success("评论成功");
        }

        /// <summary>
        /// 审核评论-拉黑/取消拉黑
        /// </summary>
        /// <param name="id">审论Id</param>
        /// <param name="isAudit"></param>
        /// <returns></returns>
        [LinCmsAuthorize("审核评论", "评论")]
        [HttpPut("{id}")]
        public ResultDto Put(Guid id, bool isAudit)
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


    }
}