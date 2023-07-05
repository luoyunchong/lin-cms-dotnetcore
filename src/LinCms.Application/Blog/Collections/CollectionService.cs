using FreeSql;
using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Blog.Collections;
using LinCms.Entities.Blog;
using LinCms.Exceptions;
using LinCms.Security;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LinCms.Blog.Comments;

/// <summary>
/// 收藏服务
/// </summary>
public class CollectionService : CrudAppService<Collection, CollectionDto, CollectionDto, Guid, CollectionSearchDto, CreateUpdateCollectionDto, CreateUpdateCollectionDto>, ICollectionService
{
    #region Constructor
    private readonly IAuditBaseRepository<Collection, Guid> _collectionRepsitory;
    private readonly IAuditBaseRepository<ArticleCollection> _artCollectionRepsitory;

    public CollectionService(IAuditBaseRepository<Collection, Guid> collectionRepsitory, IAuditBaseRepository<ArticleCollection> artCollectionRepsitory) : base(collectionRepsitory)
    {
        _collectionRepsitory = collectionRepsitory;
        _artCollectionRepsitory = artCollectionRepsitory;
    }
    #endregion

    protected override ISelect<Collection> CreateFilteredQuery(CollectionSearchDto input)
    {
        Expression<Func<Collection, bool>> exp = u => false;
        if (input.UserId != null)
        {
            if (input.UserId == CurrentUser.FindUserId())
            {
                exp = u => u.CreateUserId == input.UserId;
            }
            else
            {
                exp = exp.And(r => r.PrivacyType == PrivacyType.Public);
            }
        }
        return base.CreateFilteredQuery(input)
            .Where(exp)
            .WhereIf(input.Name.IsNotNullOrEmpty(), r => r.Name.Contains(input.Name));
    }

    /// <inheritdoc/>
    [Transactional]
    public override async Task DeleteAsync(Guid collectionId)
    {
        Collection collection = await _collectionRepsitory.Where(r => r.Id == collectionId).FirstAsync();
        if (collection.CreateUserId != CurrentUser.FindUserId())
        {
            throw new LinCmsException("您只可删除自己创建的收藏集");
        }
        await _artCollectionRepsitory.DeleteAsync(r => r.CollectionId == collectionId);
        await _collectionRepsitory.DeleteAsync(r => r.Id == collectionId);
    }
}