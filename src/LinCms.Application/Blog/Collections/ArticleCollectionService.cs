using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Blog.Articles;
using LinCms.Blog.Collections;
using LinCms.Entities.Blog;
using LinCms.Security;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LinCms.Blog.Comments;

/// <summary>
/// 文章收藏服务
/// </summary>
public class ArticleCollectionService : ApplicationService, IArticleCollectionService
{
    private readonly IAuditBaseRepository<ArticleCollection> _artCollectionRepository;
    private readonly IArticleService _articleService;

    public ArticleCollectionService(IAuditBaseRepository<ArticleCollection> collectionRepository,
        IArticleService articleService)
    {
        _artCollectionRepository = collectionRepository;
        _articleService = articleService;
    }

    /// <summary>
    /// 收藏文章或取消收藏文章
    /// </summary>
    /// <param name="crDto"></param>
    /// <returns></returns>
    [Transactional]
    public async Task<bool> CreateOrCancelAsync(CreateCancelArticleCollectionDto crDto)
    {
        Expression<Func<ArticleCollection, bool>> predicate = r =>
            r.CollectionId == crDto.CollectionId && r.ArticleId == crDto.ArticleId &&
            r.CreateUserId == CurrentUser.FindUserId();
        int increaseLikeQuantity = 1;

        bool exist = await _artCollectionRepository.Select.AnyAsync(predicate);
        if (exist)
        {
            increaseLikeQuantity = -1;
            await _artCollectionRepository.DeleteAsync(predicate);
        }
        else
        {
            ArticleCollection articleCollection = Mapper.Map<ArticleCollection>(crDto);
            await _artCollectionRepository.InsertAsync(articleCollection);
        }
        await _articleService.UpdateCollectQuantityAysnc(crDto.ArticleId, increaseLikeQuantity);

        return increaseLikeQuantity > 0;
    }
}