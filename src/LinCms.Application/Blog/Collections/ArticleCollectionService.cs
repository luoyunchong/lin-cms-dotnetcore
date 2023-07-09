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
    private readonly IAuditBaseRepository<Collection> _collectionRepository;

    public ArticleCollectionService(IAuditBaseRepository<ArticleCollection> collectionRepository,
        IArticleService articleService, IAuditBaseRepository<Collection> collectionRepository1)
    {
        _artCollectionRepository = collectionRepository;
        _articleService = articleService;
        _collectionRepository = collectionRepository1;
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
            r.ArticleId == crDto.ArticleId && r.CreateUserId == CurrentUser.FindUserId();
    
        int increaseCollectionQuantity = 1;

        ArticleCollection articleCollection = await _artCollectionRepository.Select.Where(predicate).FirstAsync();
        if (articleCollection != null)
        {
            increaseCollectionQuantity = -1;
            await _artCollectionRepository.DeleteAsync(predicate);
        }
        else
        {
            articleCollection = Mapper.Map<ArticleCollection>(crDto);
            await _artCollectionRepository.InsertAsync(articleCollection);
        }

        await _articleService.UpdateCollectQuantityAysnc(crDto.ArticleId, increaseCollectionQuantity);

        //更新收藏的数量
        Collection collection = await _collectionRepository
            .Where(r => r.Id == articleCollection.CollectionId)
            .FirstAsync();
        if (collection == null) return increaseCollectionQuantity > 0;
        collection.UpdateQuantity(increaseCollectionQuantity);
        await _collectionRepository.UpdateAsync(collection);

        return increaseCollectionQuantity > 0;
    }
}