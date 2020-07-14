using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DotNetCore.CAP;
using FreeSql;
using LinCms.Application.Contracts.Blog.Comments;
using LinCms.Application.Contracts.Blog.Comments.Dtos;
using LinCms.Application.Contracts.Blog.Notifications.Dtos;
using LinCms.Application.Contracts.Cms.Users.Dtos;
using LinCms.Core.Aop.Filter;
using LinCms.Core.Data;
using LinCms.Core.Entities.Blog;
using LinCms.Core.Exceptions;
using LinCms.Core.Extensions;
using LinCms.Core.IRepositories;
using LinCms.Core.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.Blog
{
    /// <summary>
    /// 随笔评论
    /// </summary>
    [Area("blog")]
    [Route("api/blog/comments")]
    [ApiController]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly IAuditBaseRepository<Comment> _commentAuditBaseRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        private readonly ICommentService _commentService;
        private readonly IAuditBaseRepository<Article> _articleRepository;
        private readonly ICapPublisher _capBus;
        private readonly IFileRepository _fileRepository;
        private readonly UnitOfWorkManager _unitOfWorkManager;

        public CommentController(
            IAuditBaseRepository<Comment> commentAuditBaseRepository,
            IMapper mapper,
            ICurrentUser currentUser, ICommentService commentService,
            IAuditBaseRepository<Article> articleRepository, ICapPublisher capBus, IFileRepository fileRepository,
            UnitOfWorkManager unitOfWorkManager)
        {
            _commentAuditBaseRepository = commentAuditBaseRepository;
            _mapper = mapper;
            _currentUser = currentUser;
            _commentService = commentService;
            _articleRepository = articleRepository;
            _capBus = capBus;
            _fileRepository = fileRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        /// <summary>
        /// 评论分页列表页,当随笔Id有值时，查询出此随笔的评论
        /// </summary>
        /// <param name="commentSearchDto"></param>
        /// <returns></returns>
        [HttpGet("public")]
        [AllowAnonymous]
        public Task<PagedResultDto<CommentDto>> GetListByArticleAsync([FromQuery] CommentSearchDto commentSearchDto)
        {
            return _commentService.GetListByArticleAsync(commentSearchDto);
        }

        /// <summary>
        /// 后台权限-评论列表页
        /// </summary>
        /// <param name="commentSearchDto"></param>
        /// <returns></returns>
        [HttpGet]
        [LinCmsAuthorize("评论列表", "评论")]
        public Task<PagedResultDto<CommentDto>> GetListAsync([FromQuery] CommentSearchDto commentSearchDto)
        {
            return _commentService.GetListAsync(commentSearchDto);
        }

        /// <summary>
        /// 后台权限-删除评论
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("cms/{id}")]
        [LinCmsAuthorize("删除评论", "评论")]
        public async Task<UnifyResponseDto> DeleteAsync(Guid id)
        {
            Comment comment = _commentAuditBaseRepository.Select.Where(r => r.Id == id).First();
            await _commentService.DeleteAsync(comment);
            return UnifyResponseDto.Success();
        }

        /// <summary>
        /// 用户仅可删除自己的评论
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<UnifyResponseDto> DeleteMyComment(Guid id)
        {
            Comment comment = await _commentAuditBaseRepository.Select.Where(r => r.Id == id).FirstAsync();
            if (comment == null)
            {
                return UnifyResponseDto.Error("该评论已删除");
            }

            if (comment.CreateUserId != _currentUser.Id)
            {
                return UnifyResponseDto.Error("无权限删除他人的评论");
            }

            using (IUnitOfWork uow = _unitOfWorkManager.Begin())
            {
                using ICapTransaction trans = uow.BeginTransaction(_capBus, false);

                await _commentService.DeleteAsync(comment);

                await _capBus.PublishAsync("NotificationController.Post", new CreateNotificationDto()
                {
                    NotificationType = NotificationType.UserCommentOnArticle,
                    ArticleId = comment.SubjectId,
                    UserInfoId = (long)_currentUser.Id,
                    CommentId = comment.Id,
                    IsCancel = true
                });

                await trans.CommitAsync();
            }

            return UnifyResponseDto.Success();
        }

        /// <summary>
        /// 用户评论
        /// </summary>
        /// <param name="createCommentDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<UnifyResponseDto> CreateAsync([FromBody] CreateCommentDto createCommentDto)
        {
            using IUnitOfWork uow = _unitOfWorkManager.Begin();
            using ICapTransaction trans = uow.BeginTransaction(_capBus, false);

            Comment comment = _mapper.Map<Comment>(createCommentDto);
            await _commentAuditBaseRepository.InsertAsync(comment);

            if (createCommentDto.RootCommentId.HasValue)
            {
                await _commentAuditBaseRepository.UpdateDiy
                    .Set(r => r.ChildsCount + 1)
                    .Where(r => r.Id == createCommentDto.RootCommentId)
                    .ExecuteAffrowsAsync();
            }

            switch (createCommentDto.SubjectType)
            {
                case 1:
                    await _articleRepository.UpdateDiy
                        .Set(r => r.CommentQuantity + 1)
                        .Where(r => r.Id == createCommentDto.SubjectId)
                        .ExecuteAffrowsAsync();
                    break;
            }

            if (_currentUser.Id != createCommentDto.RespUserId)
            {
                await _capBus.PublishAsync("NotificationController.Post", new CreateNotificationDto()
                {
                    NotificationType = NotificationType.UserCommentOnArticle,
                    ArticleId = createCommentDto.SubjectId,
                    NotificationRespUserId = createCommentDto.RespUserId,
                    UserInfoId = _currentUser.Id ?? 0,
                    CreateTime = comment.CreateTime,
                    CommentId = comment.Id
                });
            }

            await trans.CommitAsync();

            return UnifyResponseDto.Success("评论成功");
        }

        /// <summary>
        /// 审核评论-拉黑/取消拉黑
        /// </summary>
        /// <param name="id">审论Id</param>
        /// <param name="isAudit"></param>
        /// <returns></returns>
        [LinCmsAuthorize("审核评论", "评论")]
        [HttpPut("{id}")]
        public async Task<UnifyResponseDto> UpdateAsync(Guid id, bool isAudit)
        {
            Comment comment = _commentAuditBaseRepository.Select.Where(r => r.Id == id).ToOne();
            if (comment == null)
            {
                throw new LinCmsException("没有找到相关评论");
            }

            comment.IsAudit = isAudit;
            await _commentAuditBaseRepository.UpdateAsync(comment);
            return UnifyResponseDto.Success();
        }

        /// <summary>
        /// 评论-校正评论量,subjectType(1：文章)
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="subjectType"></param>
        /// <returns></returns>
        [LinCmsAuthorize("校正评论量", "评论")]
        [HttpPut("{subjectId}/type/${subject_type}")]
        public UnifyResponseDto CorrectedComment(Guid subjectId, int subjectType)
        {
            long count = _commentAuditBaseRepository.Select.Where(r => r.SubjectId == subjectId).Count();

            switch (subjectType)
            {
                case 1:
                    _articleRepository.UpdateDiy.Set(r => r.CommentQuantity, count).Where(r => r.Id == subjectId)
                        .ExecuteAffrows();
                    break;
            }

            return UnifyResponseDto.Success();
        }
    }
}