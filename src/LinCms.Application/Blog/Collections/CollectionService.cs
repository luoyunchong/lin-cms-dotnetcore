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
public class CollectionService(IAuditBaseRepository<Collection, Guid> collectionRepsitory,
        IAuditBaseRepository<ArticleCollection> artCollectionRepsitory)
    :
        CrudAppService<Collection, CollectionDto, CollectionDto, Guid, CollectionSearchDto, CreateUpdateCollectionDto,
            CreateUpdateCollectionDto>(collectionRepsitory), ICollectionService
{
    #region Constructor

    #endregion


    public override async Task<CollectionDto> UpdateAsync(Guid id, CreateUpdateCollectionDto updateInput)
    {
        Collection entity = await GetEntityByIdAsync(id);
        if (entity.CreateUserId != CurrentUser.FindUserId())
        {
            throw new LinCmsException("您只可修改自己创建的收藏集");
        }

        Mapper.Map(updateInput, entity);
        await Repository.UpdateAsync(entity);
        return Mapper.Map<CollectionDto>(entity);
    }

    protected override ISelect<Collection> CreateFilteredQuery(CollectionSearchDto input)
    {
        Expression<Func<Collection, bool>> exp = u => true;
        if (input.UserId != null)
        {
            if (input.UserId == CurrentUser.FindUserId())
            {
                exp = u => u.CreateUserId == input.UserId;
            }
            else
            {
                exp = exp.And(r => r.CreateUserId == input.UserId && r.PrivacyType == PrivacyType.Public);
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
        Collection collection = await collectionRepsitory.Where(r => r.Id == collectionId).FirstAsync();
        if (collection.CreateUserId != CurrentUser.FindUserId())
        {
            throw new LinCmsException("您只可删除自己创建的收藏集");
        }

        await artCollectionRepsitory.DeleteAsync(r => r.CollectionId == collectionId);
        await collectionRepsitory.DeleteAsync(r => r.Id == collectionId);
    }

    public override async Task<CollectionDto> GetAsync(Guid id)
    {
        var collectionDto = await base.GetAsync(id);
        if (collectionDto.PrivacyType == PrivacyType.VisibleOnlyMySelf)
        {
            if (collectionDto.CreateUserId != CurrentUser.FindUserId())
            {
                throw new LinCmsException($"作者设置了仅自己可见");
            }
        }
        return collectionDto;
    }
}