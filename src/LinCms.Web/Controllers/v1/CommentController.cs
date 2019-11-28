using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LinCms.Web.Models.v1.Comments;
using LinCms.Web.Services.Cms.Interfaces;
using LinCms.Web.Services.v1.Interfaces;
using LinCms.Zero.Aop;
using LinCms.Zero.Data;
using LinCms.Zero.Domain.Blog;
using LinCms.Zero.Exceptions;
using LinCms.Zero.Extensions;
using LinCms.Zero.Repositories;
using LinCms.Zero.Security;
using Microsoft.AspNetCore.Authorization;
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
        private readonly ICommentService _commentService;
        private readonly AuditBaseRepository<Article> _articleRepository;

        public CommentController(AuditBaseRepository<Comment> commentAuditBaseRepository, IMapper mapper,
            ICurrentUser currentUser, IUserSevice userService, ICommentService commentService,
            AuditBaseRepository<Article> articleRepository)
        {
            _commentAuditBaseRepository = commentAuditBaseRepository;
            _mapper = mapper;
            _currentUser = currentUser;
            _userService = userService;
            _commentService = commentService;
            _articleRepository = articleRepository;
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
                .Where(r => r.RootCommentId == commentSearchDto.RootCommentId && r.IsAudited == true)
                .OrderByDescending(!commentSearchDto.RootCommentId.HasValue, r => r.CreateTime)
                .OrderBy(commentSearchDto.RootCommentId.HasValue, r => r.CreateTime)
                .ToPagerList(commentSearchDto, out long totalCount)
                .Select(r =>
                {
                    CommentDto commentDto = _mapper.Map<CommentDto>(r);

                    if (commentDto.UserInfo != null)
                    {
                        commentDto.UserInfo.Avatar = _currentUser.GetFileUrl(commentDto.UserInfo.Avatar);
                    }

                    commentDto.TopComment = r.Childs.ToList().Select(u =>
                    {
                        CommentDto childrenDto = _mapper.Map<CommentDto>(u);
                        if (childrenDto.UserInfo != null)
                        {
                            childrenDto.UserInfo.Avatar = _currentUser.GetFileUrl(childrenDto.UserInfo.Avatar);
                        }

                        childrenDto.IsLiked =
                            userId != null && u.UserLikes.Where(z => z.CreateUserId == userId).IsNotEmpty();
                        return childrenDto;
                    }).ToList();
                    commentDto.IsLiked =
                        userId != null && r.UserLikes.Where(u => u.CreateUserId == userId).IsNotEmpty();
                    return commentDto;
                }).ToList();
            
            //重新计算一个文章多个评论
            totalCount = GetCommentCount(commentSearchDto);
            
            return new PagedResultDto<CommentDto>(comments, totalCount);
        }

        private long GetCommentCount(CommentSearchDto commentSearchDto)
        {
            return _commentAuditBaseRepository
                 .Select
                 .WhereCascade(x => x.IsDeleted == false)
                 .Where(r => r.IsAudited == true && r.SubjectId == commentSearchDto.SubjectId).Count();
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

        [HttpDelete("cms/{id}")]
        [LinCmsAuthorize("删除评论", "评论")]
        public ResultDto Delete(Guid id)
        {
            Comment comment = _commentAuditBaseRepository.Select.Where(r => r.Id == id).First();
            _commentService.Delete(comment);
            return ResultDto.Success();
        }

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
                    _articleRepository.UpdateDiy.Set(r => r.CommentQuantity + 1)
                        .Where(r => r.Id == createCommentDto.SubjectId).ExecuteAffrows();
                    break;
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
        [HttpPut("audit/{id}")]
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

        /// <summary>
        /// 评论-校正评论量,id->subject_id,type->subject_type(1：文章)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [LinCmsAuthorize("校正评论量", "评论")]
        [HttpPut("{id}/type/${type}")]
        public ResultDto CorrectedComment(Guid id, int type)
        {
            long count = _commentAuditBaseRepository.Select.Where(r => r.SubjectId == id).Count();

            switch (type)
            {
                case 1:
                    _articleRepository.UpdateDiy.Set(r => r.CommentQuantity, count).Where(r => r.Id == id).ExecuteAffrows();
                    break;
            }
            return ResultDto.Success();
        }

    }
}