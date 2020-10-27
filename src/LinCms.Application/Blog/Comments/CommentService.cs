using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;
using FreeSql;
using LinCms.Aop.Attributes;
using LinCms.Blog.Notifications;
using LinCms.Cms.Users;
using LinCms.Data;
using LinCms.Entities.Blog;
using LinCms.Exceptions;
using LinCms.Extensions;
using LinCms.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Blog.Comments
{
    public class CommentService : ApplicationService, ICommentService
    {
        private readonly IAuditBaseRepository<Comment> _commentRepository;
        private readonly IAuditBaseRepository<Article> _articleRepository;
        private readonly IFileRepository _fileRepository;
        private readonly ICapPublisher _capBus;

        public CommentService(IAuditBaseRepository<Comment> commentRepository,
            IAuditBaseRepository<Article> articleRepository,
            IFileRepository fileRepository, ICapPublisher capBus)
        {
            _commentRepository = commentRepository;
            _articleRepository = articleRepository;
            _fileRepository = fileRepository;
            _capBus = capBus;
        }

        public async Task<PagedResultDto<CommentDto>> GetListByArticleAsync([FromQuery] CommentSearchDto commentSearchDto)
        {
            long? userId = CurrentUser.Id;
            List<CommentDto> comments = (await _commentRepository
                    .Select
                    .Include(r => r.UserInfo)
                    .Include(r => r.RespUserInfo)
                    .IncludeMany(r => r.Childs, t => t.Include(u => u.UserInfo).Include(u => u.RespUserInfo).IncludeMany(u => u.UserLikes))
                    .IncludeMany(r => r.UserLikes)
                    .WhereCascade(x => x.IsDeleted == false)
                    .WhereIf(commentSearchDto.SubjectId.HasValue, r => r.SubjectId == commentSearchDto.SubjectId)
                    .Where(r => r.RootCommentId == commentSearchDto.RootCommentId) // && r.IsAudit == true
                    .OrderByDescending(!commentSearchDto.RootCommentId.HasValue, r => r.CreateTime)
                    .OrderBy(commentSearchDto.RootCommentId.HasValue, r => r.CreateTime)
                    .Page(commentSearchDto.Page + 1, commentSearchDto.Count).ToListAsync())
                //.ToPagerList(commentSearchDto, out long totalCount)
                .Select(r =>
                {
                    CommentDto commentDto = Mapper.Map<CommentDto>(r);
                    if (commentDto.IsAudit == false)
                    {
                        commentDto.Text = "[该评论因违规被拉黑]";
                    }

                    if (commentDto.UserInfo == null)
                    {
                        commentDto.UserInfo = new OpenUserDto("该用户已被系统删除");
                    }
                    else
                    {
                        commentDto.UserInfo.Avatar = _fileRepository.GetFileUrl(commentDto.UserInfo.Avatar);
                    }


                    commentDto.IsLiked =
                        userId != null && r.UserLikes.Where(u => u.CreateUserId == userId).IsNotEmpty();

                    commentDto.TopComment = r.Childs.ToList().Select(u =>
                    {
                        CommentDto childrenDto = Mapper.Map<CommentDto>(u);
                        if (childrenDto.UserInfo != null)
                        {
                            childrenDto.UserInfo.Avatar = _fileRepository.GetFileUrl(childrenDto.UserInfo.Avatar);
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
            return _commentRepository
                .Select
                .Where(r => r.IsDeleted == false && r.SubjectId == commentSearchDto.SubjectId).Count();
        }

        public async Task<PagedResultDto<CommentDto>> GetListAsync([FromQuery] CommentSearchDto commentSearchDto)
        {
            List<CommentDto> comments = (await _commentRepository
                    .Select
                    .Include(r => r.UserInfo)
                    .WhereIf(commentSearchDto.SubjectId.HasValue, r => r.SubjectId == commentSearchDto.SubjectId)
                    .WhereIf(commentSearchDto.Text.IsNotNullOrEmpty(), r => r.Text.Contains(commentSearchDto.Text))
                    .OrderByDescending(r => r.CreateTime)
                    .ToPagerListAsync(commentSearchDto, out long totalCount)
                )
                .Select(r =>
                {
                    CommentDto commentDto = Mapper.Map<CommentDto>(r);

                    if (commentDto.UserInfo != null)
                    {
                        commentDto.UserInfo.Avatar = _fileRepository.GetFileUrl(commentDto.UserInfo.Avatar);
                    }

                    return commentDto;
                }).ToList();

            return new PagedResultDto<CommentDto>(comments, totalCount);
        }

        [Transactional]
        public async Task CreateAsync(CreateCommentDto createCommentDto)
        {

            Comment comment = Mapper.Map<Comment>(createCommentDto);
            await _commentRepository.InsertAsync(comment);

            if (createCommentDto.RootCommentId.HasValue)
            {
                await _commentRepository.UpdateDiy
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

            if (CurrentUser.Id != createCommentDto.RespUserId)
            {
                using ICapTransaction capTransaction = UnitOfWorkManager.Current.BeginTransaction(_capBus, false);
                await _capBus.PublishAsync(CreateNotificationDto.CreateOrCancelAsync, new CreateNotificationDto()
                {
                    NotificationType = NotificationType.UserCommentOnArticle,
                    ArticleId = createCommentDto.SubjectId,
                    NotificationRespUserId = createCommentDto.RespUserId,
                    UserInfoId = CurrentUser.Id ?? 0,
                    CreateTime = comment.CreateTime,
                    CommentId = comment.Id
                });
                capTransaction.Commit(UnitOfWorkManager.Current);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            Comment comment = _commentRepository.Select.Where(r => r.Id == id).First();
            await DeleteAsync(comment);
        }

        /// <summary>
        /// 删除评论并同步随笔数量
        /// </summary>
        /// <param name="comment"></param>
        [Transactional]
        public async Task DeleteAsync(Comment comment)
        {
            int affrows = 0;
            //如果是根评论，删除所有的子评论
            if (!comment.RootCommentId.HasValue)
            {
                affrows += await _commentRepository.DeleteAsync(r => r.RootCommentId == comment.Id);
            }
            else
            {
                await _commentRepository.UpdateDiy.Set(r => r.ChildsCount - 1).Where(r => r.Id == comment.RootCommentId)
                    .ExecuteAffrowsAsync();
            }

            affrows += await _commentRepository.DeleteAsync(new Comment { Id = comment.Id });

            switch (comment.SubjectType)
            {
                case 1:
                    await _articleRepository.UpdateDiy.Set(r => r.CommentQuantity - affrows)
                        .Where(r => r.Id == comment.SubjectId).ExecuteAffrowsAsync();
                    break;
            }
        }

        [Transactional]
        public async Task DeleteMyComment(Guid id)
        {
            Comment comment = await _commentRepository.Select.Where(r => r.Id == id).FirstAsync();
            if (comment == null)
            {
                throw new LinCmsException("该评论已删除");
            }

            if (comment.CreateUserId != CurrentUser.Id)
            {
                throw new LinCmsException("无权限删除他人的评论");
            }

            using ICapTransaction capTransaction = UnitOfWorkManager.Current.BeginTransaction(_capBus, false);

            await this.DeleteAsync(comment);

            await _capBus.PublishAsync(CreateNotificationDto.CreateOrCancelAsync, new CreateNotificationDto()
            {
                NotificationType = NotificationType.UserCommentOnArticle,
                ArticleId = comment.SubjectId,
                UserInfoId = (long)CurrentUser.Id,
                CommentId = comment.Id,
                IsCancel = true
            });
            capTransaction.Commit(UnitOfWorkManager.Current);
        }

        public async Task UpdateLikeQuantityAysnc(Guid subjectId, int likesQuantity)
        {
            Comment comment = await _commentRepository.Select.Where(r => r.Id == subjectId).ToOneAsync();
            comment.UpdateLikeQuantity(likesQuantity);
            await _commentRepository.UpdateAsync(comment);
        }
    }
}