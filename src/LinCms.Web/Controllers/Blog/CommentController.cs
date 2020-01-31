using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DotNetCore.CAP;
using LinCms.Application.Blog.Comments;
using LinCms.Application.Cms.Users;
using LinCms.Application.Contracts.Blog.Comments;
using LinCms.Application.Contracts.Blog.Notifications;
using LinCms.Core.Aop;
using LinCms.Core.Data;
using LinCms.Core.Entities.Blog;
using LinCms.Core.Exceptions;
using LinCms.Core.Extensions;
using LinCms.Core.Security;
using LinCms.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.Blog
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
        private readonly ICommentService _commentService;
        private readonly ICapPublisher _capBus;
        private readonly AuditBaseRepository<Article> _articleRepository;

        public CommentController(AuditBaseRepository<Comment> commentAuditBaseRepository, IMapper mapper,
            ICurrentUser currentUser, IUserSevice userService, ICommentService commentService,
            AuditBaseRepository<Article> articleRepository, ICapPublisher capBus)
        {
            _commentAuditBaseRepository = commentAuditBaseRepository;
            _mapper = mapper;
            _currentUser = currentUser;
            _userService = userService;
            _commentService = commentService;
            _articleRepository = articleRepository;
            _capBus = capBus;
        }

        /// <summary>
        /// 评论分页列表页,当随笔Id有值时，查询出此随笔的评论
        /// </summary>
        /// <param name="commentSearchDto"></param>
        /// <returns></returns>
        [HttpGet("public")]
        [AllowAnonymous]
        public PagedResultDto<CommentDto> Get([FromQuery] CommentSearchDto commentSearchDto)
        {
            long? userId = _currentUser.Id;
            List<CommentDto> comments = _commentAuditBaseRepository
                .Select
                .Include(r => r.UserInfo)
                .Include(r => r.RespUserInfo)
                .IncludeMany(r => r.Childs, //.Take(2)
                    t => t.Include(u => u.UserInfo).Include(u => u.RespUserInfo).IncludeMany(u => u.UserLikes))
                .IncludeMany(r => r.UserLikes)
                .WhereCascade(x => x.IsDeleted == false)
                .WhereIf(commentSearchDto.SubjectId.HasValue, r => r.SubjectId == commentSearchDto.SubjectId)
                .Where(r => r.RootCommentId == commentSearchDto.RootCommentId)// && r.IsAudit == true
                .OrderByDescending(!commentSearchDto.RootCommentId.HasValue, r => r.CreateTime)
                .OrderBy(commentSearchDto.RootCommentId.HasValue, r => r.CreateTime)
                .Page(commentSearchDto.Page + 1, commentSearchDto.Count).ToList()
                //.ToPagerList(commentSearchDto, out long totalCount)
                .Select(r =>
                {
                    CommentDto commentDto = _mapper.Map<CommentDto>(r);
                    if (commentDto.IsAudit == false)
                    {
                        commentDto.Text = "[该评论因违规被拉黑]";
                    }
                    if (commentDto.UserInfo != null)
                    {
                        commentDto.UserInfo.Avatar = _currentUser.GetFileUrl(commentDto.UserInfo.Avatar);
                    }

                    commentDto.IsLiked =
                        userId != null && r.UserLikes.Where(u => u.CreateUserId == userId).IsNotEmpty();

                    commentDto.TopComment = r.Childs.ToList().Select(u =>
                    {
                        CommentDto childrenDto = _mapper.Map<CommentDto>(u);
                        if (childrenDto.UserInfo != null)
                        {
                            childrenDto.UserInfo.Avatar = _currentUser.GetFileUrl(childrenDto.UserInfo.Avatar);
                        }

                        if (childrenDto.IsAudit == false)
                        {
                            childrenDto.Text = "[该评论因违规被拉黑]";
                        }

                        childrenDto.IsLiked =
                            userId != null && u.UserLikes.Where(z => z.CreateUserId == userId).IsNotEmpty();
                        return childrenDto;
                    }).ToList();
                    return commentDto;
                }).ToList();

            //计算一个文章多少个评论
            long totalCount = GetCommentCount(commentSearchDto);

            return new PagedResultDto<CommentDto>(comments, totalCount);
        }

        private long GetCommentCount(CommentSearchDto commentSearchDto)
        {
            return _commentAuditBaseRepository
                 .Select
                 .Where(r => r.IsDeleted == false && r.SubjectId == commentSearchDto.SubjectId).Count();
        }

        /// <summary>
        /// 查询出此随笔的评论
        /// </summary>
        /// <param name="commentSearchDto"></param>
        /// <returns></returns>
        [HttpGet]
        [LinCmsAuthorize("评论列表", "评论")]
        public PagedResultDto<CommentDto> GetAll([FromQuery] CommentSearchDto commentSearchDto)
        {
            List<CommentDto> comments = _commentAuditBaseRepository
                .Select
                .Include(r => r.UserInfo)
                .WhereIf(commentSearchDto.SubjectId.HasValue, r => r.SubjectId == commentSearchDto.SubjectId)
                .WhereIf(commentSearchDto.Text.IsNotNullOrEmpty(), r => r.Text.Contains(commentSearchDto.Text))
                .OrderByDescending(r => r.CreateTime)
                .ToPagerList(commentSearchDto, out long totalCount)
                .Select(r =>
                {
                    CommentDto commentDto = _mapper.Map<CommentDto>(r);

                    if (commentDto.UserInfo != null)
                    {
                        commentDto.UserInfo.Avatar = _currentUser.GetFileUrl(commentDto.UserInfo.Avatar);
                    }
                    return commentDto;
                }).ToList();

            return new PagedResultDto<CommentDto>(comments, totalCount);
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("cms/{id}")]
        [LinCmsAuthorize("删除评论", "评论")]
        public ResultDto Delete(Guid id)
        {
            Comment comment = _commentAuditBaseRepository.Select.Where(r => r.Id == id).First();
            _commentService.Delete(comment);
            return ResultDto.Success();
        }

        /// <summary>
        /// 用户仅可删除自己的评论
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public ResultDto DeleteMyComment(Guid id)
        {
            Comment comment = _commentAuditBaseRepository.Select.Where(r => r.Id == id).First();
            if (comment == null)
            {
                return ResultDto.Error("该评论已删除");
            }
            if (comment.CreateUserId != _currentUser.Id)
            {
                return ResultDto.Error("无权限删除他人的评论");
            }
            _commentService.Delete(comment);
            return ResultDto.Success();

        }

        /// <summary>
        /// 用户评论
        /// </summary>
        /// <param name="createCommentDto"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto Post([FromBody] CreateCommentDto createCommentDto)
        {
            Comment comment = _mapper.Map<Comment>(createCommentDto);
            _commentAuditBaseRepository.Insert(comment);

            if (createCommentDto.RootCommentId.HasValue)
            {
                _commentAuditBaseRepository.UpdateDiy.Set(r => r.ChildsCount + 1).Where(r => r.Id == createCommentDto.RootCommentId).ExecuteAffrows();
            }

            switch (createCommentDto.SubjectType)
            {
                case 1:
                    _articleRepository.UpdateDiy
                        .Set(r => r.CommentQuantity + 1)
                        .Where(r => r.Id == createCommentDto.SubjectId)
                        .ExecuteAffrows();
                    break;
            }

            if (_currentUser.Id != createCommentDto.RespUserId)
            {
                _capBus.Publish("NotificationController.Post", new CreateNotificationDto()
                {
                    NotificationType = NotificationType.UserCommentOnArticle,
                    ArticleId = createCommentDto.SubjectId,
                    NotificationRespUserId = createCommentDto.RespUserId,
                    UserInfoId = _currentUser.Id ?? 0,
                    CreateTime = comment.CreateTime,
                    CommentId = comment.Id
                });
            }

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

            comment.IsAudit = isAudit;
            _commentAuditBaseRepository.Update(comment);
            return ResultDto.Success();
        }

        /// <summary>
        /// 评论-校正评论量,subjectType(1：文章)
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="subjectType"></param>
        /// <returns></returns>
        [LinCmsAuthorize("校正评论量", "评论")]
        [HttpPut("{subjectId}/type/${subject_type}")]
        public ResultDto CorrectedComment(Guid subjectId, int subjectType)
        {
            long count = _commentAuditBaseRepository.Select.Where(r => r.SubjectId == subjectId).Count();

            switch (subjectType)
            {
                case 1:
                    _articleRepository.UpdateDiy.Set(r => r.CommentQuantity, count).Where(r => r.Id == subjectId).ExecuteAffrows();
                    break;
            }
            return ResultDto.Success();
        }

    }
}