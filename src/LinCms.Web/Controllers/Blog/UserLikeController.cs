using System;
using System.Threading.Tasks;
using DotNetCore.CAP;
using FreeSql;
using IGeekFan.FreeKit.Extras.FreeSql;
using IGeekFan.FreeKit.Extras.Security;
using LinCms.Blog.Notifications;
using LinCms.Blog.UserLikes;
using LinCms.Data;
using LinCms.Entities.Blog;
using LinCms.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Controllers.Blog;

/// <summary>
///  用户点赞随笔
/// </summary>
[ApiExplorerSettings(GroupName = "blog")]
[Area("blog")]
[Route("api/blog/user-like")]
[ApiController]
[Authorize]
public class UserLikeController(ICurrentUser currentUser,
        IAuditBaseRepository<Article> articleRepository,
        IAuditBaseRepository<Comment> commentRepository,
        ICapPublisher capBus,
        UnitOfWorkManager unitOfWorkManager,
        IUserLikeService userLikeService)
    : ApiControllerBase
{
    /// <summary>
    /// 用户点赞/取消点赞随笔、评论 
    /// </summary>
    /// <param name="createUpdateUserLike"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<UnifyResponseDto> CreateOrCancelAsync([FromBody] CreateUpdateUserLikeDto createUpdateUserLike)
    {
        IUnitOfWork unitOfWork = unitOfWorkManager.Begin();
        using ICapTransaction capTransaction = unitOfWork.BeginTransaction(capBus, false);

        bool isCancel = await userLikeService.CreateOrCancelAsync(createUpdateUserLike);

        await PublishUserLikeNotification(createUpdateUserLike, isCancel);

        capTransaction.Commit(unitOfWork);

        return UnifyResponseDto.Success(isCancel == false ? "点赞成功" : "已取消点赞");
    }

    /// <summary>
    /// 根据用户点赞类型：随笔、评论，得到消息的NotificationRespUserId的值
    /// </summary>
    /// <param name="createUpdateUserLike"></param>
    /// <param name="isCancel"></param>
    /// <returns></returns>
    private async Task PublishUserLikeNotification(CreateUpdateUserLikeDto createUpdateUserLike, bool isCancel)
    {
        var createNotificationDto = new CreateNotificationDto()
        {
            UserInfoId = currentUser.FindUserId() ?? 0,
            CreateTime = DateTime.Now,
            IsCancel = isCancel
        };

        switch (createUpdateUserLike.SubjectType)
        {
            case UserLikeSubjectType.UserLikeArticle:

                Article subjectArticle = await articleRepository.Where(r => r.Id == createUpdateUserLike.SubjectId).ToOneAsync();

                createNotificationDto.NotificationRespUserId = subjectArticle.CreateUserId.Value;
                createNotificationDto.NotificationType = NotificationType.UserLikeArticle;
                createNotificationDto.ArticleId = createUpdateUserLike.SubjectId;
                break;

            case UserLikeSubjectType.UserLikeComment:

                Comment subjectComment = await commentRepository.Where(r => r.Id == createUpdateUserLike.SubjectId).ToOneAsync();

                createNotificationDto.NotificationRespUserId = subjectComment.CreateUserId.Value;
                createNotificationDto.NotificationType = NotificationType.UserLikeArticleComment;
                createNotificationDto.ArticleId = subjectComment.SubjectId;
                createNotificationDto.CommentId = createUpdateUserLike.SubjectId;
                break;
        }

        if (createNotificationDto.NotificationRespUserId != 0 && currentUser.FindUserId() != createNotificationDto.NotificationRespUserId)
        {
            await capBus.PublishAsync(CreateNotificationDto.CreateOrCancelAsync, createNotificationDto);
        }
    }
}