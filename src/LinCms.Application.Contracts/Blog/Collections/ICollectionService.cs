using System;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dto;
using LinCms.Entities.Blog;

namespace LinCms.Blog.Collections;

public interface ICollectionService : IApplicationService
{
    Task<PagedResultDto<Collection>> GetListAsync(CollectionSearchDto input);
    Task CreateAsync(CreateUpdateCollectionDto createCollectionDto);
    Task UpdateAsync(Guid id, CreateUpdateCollectionDto upCollectionDto);
    Task DeleteAsync(Guid collectionId);
}