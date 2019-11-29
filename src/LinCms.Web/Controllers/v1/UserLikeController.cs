using System;
using System.Linq.Expressions;
using AutoMapper;
using FreeSql;
using LinCms.Web.Models.v1.UserLikes;
using LinCms.Web.Services.v1.Interfaces;
using LinCms.Zero.Data;
using LinCms.Zero.Domain.Blog;
using LinCms.Zero.Exceptions;
using LinCms.Zero.Repositories;
using LinCms.Zero.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.v1
{
    /// <summary>
    ///  用户点赞随笔
    /// </summary>
    [Route("v1/user-like")]
    [ApiController]
    [Authorize]
    public class UserLikeController : ControllerBase
    {
        private readonly AuditBaseRepository<Article> _articleAuditBaseRepository;
        private readonly AuditBaseRepository<UserLike> _userLikeRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        private readonly AuditBaseRepository<Comment> _commentRepository;
        public UserLikeController(IMapper mapper, ICurrentUser currentUser, GuidRepository<TagArticle> tagArticleRepository, IFreeSql freeSql, IArticleService articleService, AuditBaseRepository<UserLike> userLikeRepository, AuditBaseRepository<Article> articleAuditBaseRepository, AuditBaseRepository<Comment> commentRepository)
        {
            _mapper = mapper;
            _currentUser = currentUser;
            _userLikeRepository = userLikeRepository;
            _articleAuditBaseRepository = articleAuditBaseRepository;
            _commentRepository = commentRepository;
        }

        /// <summary>
        /// 用户点赞/取消点赞文章、评论 
        /// </summary>
        /// <param name="createUpdateUserLike"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultDto Post([FromBody] CreateUpdateUserLikeDto createUpdateUserLike)
        {
            Expression<Func<UserLike, bool>> predicate = r => r.SubjectId == createUpdateUserLike.SubjectId && r.CreateUserId == _currentUser.Id;

            bool exist = _userLikeRepository.Select.Any(predicate);
            if (exist)
            {
                _userLikeRepository.Delete(predicate);

                switch (createUpdateUserLike.SubjectType)
                {
                    case 1:
                        this.UpdateArticleLike(createUpdateUserLike.SubjectId, -1);
                        break;
                    case 2:
                        this.UpdateCommentLike(createUpdateUserLike.SubjectId, -1);
                        break;
                }

                return ResultDto.Success("取消点赞成功");
            }

            UserLike userLike = _mapper.Map<UserLike>(createUpdateUserLike);

            _userLikeRepository.Insert(userLike);

            switch (createUpdateUserLike.SubjectType)
            {
                case 1:
                    this.UpdateArticleLike(createUpdateUserLike.SubjectId, 1);
                    break;
                case 2:
                    this.UpdateCommentLike(createUpdateUserLike.SubjectId, 1);
                    break;
            }

            return ResultDto.Success("点赞成功");
        }

        private void UpdateArticleLike(Guid subjectId, int likesQuantity)
        {
            Article article = _articleAuditBaseRepository.Select.Where(r => r.Id == subjectId).ToOne();
            if (article.IsAudit == false)
            {
                throw new LinCmsException("该文章因违规被拉黑");
            }
            //防止数量一直减，减到小于0
            if (likesQuantity < 0)
            {
                if (article.LikesQuantity < -likesQuantity)
                {
                    return;
                }
            }
            _articleAuditBaseRepository.UpdateDiy.Set(r => r.LikesQuantity + likesQuantity).Where(r => r.Id == subjectId).ExecuteAffrows();
        }

        private void UpdateCommentLike(Guid subjectId, int likesQuantity)
        {
            Comment comment = _commentRepository.Select.Where(r => r.Id == subjectId).ToOne();
            if (comment.IsAudit == false)
            {
                throw new LinCmsException("该评论因违规被拉黑");
            }
            //防止数量一直减，减到小于0
            if (likesQuantity < 0)
            {
                if (comment.LikesQuantity < -likesQuantity)
                {
                    return;
                }
            }

            _commentRepository.UpdateDiy.Set(r => r.LikesQuantity + likesQuantity).Where(r => r.Id == subjectId).ExecuteAffrows();
        }
    }
}