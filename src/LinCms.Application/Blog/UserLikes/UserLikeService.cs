using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Blog.Articles;
using LinCms.Blog.Comments;
using LinCms.Entities.Blog;
using LinCms.Security;

namespace LinCms.Blog.UserLikes;

/// <summary>
/// 用户点赞
/// </summary>
public class UserLikeService(IAuditBaseRepository<UserLike> userLikeRepository,
        IArticleService articleService,
        ICommentService commentService)
    : ApplicationService, IUserLikeService
{
    /// <summary>
    /// 点赞：返回 false
    /// 取消点赞：返回 true
    /// </summary>
    /// <param name="createUpdateUserLike"></param>
    /// <returns></returns>
    public async Task<bool> CreateOrCancelAsync(CreateUpdateUserLikeDto createUpdateUserLike)
    {
        Expression<Func<UserLike, bool>> predicate = r => r.SubjectId == createUpdateUserLike.SubjectId && r.CreateUserId ==  CurrentUser.FindUserId();

        bool exist = await userLikeRepository.Select.AnyAsync(predicate);
        int increaseLikeQuantity = 1;

        if (exist)
        {
            increaseLikeQuantity = -1;
            await userLikeRepository.DeleteAsync(predicate);
        }

        switch (createUpdateUserLike.SubjectType)
        {
            case UserLikeSubjectType.UserLikeArticle:
                await articleService.UpdateLikeQuantityAysnc(createUpdateUserLike.SubjectId, increaseLikeQuantity);
                break;
            case UserLikeSubjectType.UserLikeComment:
                await commentService.UpdateLikeQuantityAysnc(createUpdateUserLike.SubjectId, increaseLikeQuantity);
                break;
            default:
                break;
        }

        if (exist) return true;
     

        UserLike userLike = Mapper.Map<UserLike>(createUpdateUserLike);
        await userLikeRepository.InsertAsync(userLike);
        return false;
    }
}