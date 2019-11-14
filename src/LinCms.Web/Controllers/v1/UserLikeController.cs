using System;
using System.Linq.Expressions;
using AutoMapper;
using FreeSql;
using LinCms.Web.Models.v1.UserLikes;
using LinCms.Web.Services.v1.Interfaces;
using LinCms.Zero.Data;
using LinCms.Zero.Domain.Blog;
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
        public UserLikeController( IMapper mapper, ICurrentUser currentUser, GuidRepository<TagArticle> tagArticleRepository, IFreeSql freeSql, IArticleService articleService, AuditBaseRepository<UserLike> userLikeRepository, AuditBaseRepository<Article> articleAuditBaseRepository, AuditBaseRepository<Comment> commentRepository)
        {
            _mapper = mapper;
            _currentUser = currentUser;
            _userLikeRepository = userLikeRepository;
            _articleAuditBaseRepository = articleAuditBaseRepository;
            _commentRepository = commentRepository;
        }

        /// <summary>
        /// 用户点赞/取消点赞文章 
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
                        _articleAuditBaseRepository.UpdateDiy.Set(r => r.LikesQuantity - 1).Where(r => r.Id == createUpdateUserLike.SubjectId).ExecuteAffrows();
                        break;
                    case 2:
                        _commentRepository.UpdateDiy.Set(r=>r.LikesQuantity-1).Where(r => r.Id == createUpdateUserLike.SubjectId).ExecuteAffrows();
                        break;
                }

                return ResultDto.Success("取消点赞成功");
            }

            UserLike userLike = _mapper.Map<UserLike>(createUpdateUserLike);
            
            _userLikeRepository.Insert(userLike);

            switch (createUpdateUserLike.SubjectType)
            {
                case 1:
                    _articleAuditBaseRepository.UpdateDiy.Set(r => r.LikesQuantity + 1).Where(r => r.Id == createUpdateUserLike.SubjectId).ExecuteAffrows();
                    break;
                case 2:
                    _commentRepository.UpdateDiy.Set(r => r.LikesQuantity + 1).Where(r => r.Id == createUpdateUserLike.SubjectId).ExecuteAffrows();
                    break;
            }

            return ResultDto.Success("点赞成功");
        }
    }
}