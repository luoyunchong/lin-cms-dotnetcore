using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dto;
using IGeekFan.FreeKit.Extras.FreeSql;
using LinCms.Blog.Collections;
using LinCms.Entities.Blog;
using LinCms.Exceptions;
using LinCms.Extensions;
using LinCms.Security;

namespace LinCms.Blog.Comments;

/// <summary>
/// 收藏服务
/// </summary>
public class CollectionService : ApplicationService, ICollectionService
{
    private readonly IAuditBaseRepository<Collection> _collectionRepsitory;
    private readonly IAuditBaseRepository<ArticleCollection> _artCollectionRepsitory;

    public CollectionService(IAuditBaseRepository<Collection> collectionRepsitory,
        IAuditBaseRepository<ArticleCollection> artCollectionRepsitory)
    {
        _collectionRepsitory = collectionRepsitory;
        _artCollectionRepsitory = artCollectionRepsitory;
    }

    /// <summary>
    /// 获取自己创建人收藏夹
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<PagedResultDto<Collection>> GetListAsync(CollectionSearchDto input)
    {
        List<Collection> collections = await _collectionRepsitory.Select
            .Where(r => r.CreateUserId == CurrentUser.FindUserId())
            .WhereIf(input.Name.IsNotNullOrEmpty(), r => r.Name.Contains(input.Name))
            .OrderByDescending(r => r.CreateTime)
            .ToPagerListAsync(input, out long totalCount);
        return new PagedResultDto<Collection>(collections, totalCount);
    }

    public async Task CreateAsync(CreateUpdateCollectionDto createCollectionDto)
    {
        Collection collection = Mapper.Map<Collection>(createCollectionDto);
        await _collectionRepsitory.InsertAsync(collection);
    }

    public async Task UpdateAsync(Guid id, CreateUpdateCollectionDto upCollectionDto)
    {
        Collection collection = await _collectionRepsitory.Select.Where(r => r.Id == id).ToOneAsync();
        if (collection == null)
        {
            throw new LinCmsException("该数据不存在");
        }

        Mapper.Map(upCollectionDto, collection);
        await _collectionRepsitory.UpdateAsync(collection);
    }

    [Transactional]
    public async Task DeleteAsync(Guid collectionId)
    {
        _artCollectionRepsitory.DeleteAsync(r => r.CollectionId == collectionId);
        _collectionRepsitory.DeleteAsync(r => r.Id == collectionId);
    }
}