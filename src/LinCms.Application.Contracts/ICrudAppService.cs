using System;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.AuditEntity;
using LinCms.Data;

namespace LinCms;

public interface ICrudAppService<TGetOutputDto, TGetListOutputDto, in TKey, in TGetListInput, in TCreateInput, in TUpdateInput>
    where TGetOutputDto : IEntityDto<TKey> 
    where TKey : IEquatable<TKey>
    where TGetListOutputDto : IEntityDto<TKey> 
{
    Task<PagedResultDto<TGetListOutputDto>> GetListAsync(TGetListInput input);

    Task<TGetOutputDto> GetAsync(TKey id);

    Task<TGetOutputDto> CreateAsync(TCreateInput createInput);

    Task<TGetOutputDto> UpdateAsync(TKey id, TUpdateInput updateInput);

    Task DeleteAsync(TKey id);
}