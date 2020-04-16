using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Application.Contracts.Base.BaseTypes.Dtos;
using LinCms.Core.Data;
using LinCms.Core.Entities;

namespace LinCms.Application.Contracts
{
    public interface ICrudAppService<TGetOutputDto, TGetListOutputDto,in TKey,in TGetListInput,in TCreateInput,in TUpdateInput>
        where TGetOutputDto : IEntityDto<TKey>
        where TGetListOutputDto : IEntityDto<TKey>
    {
        Task<PagedResultDto<TGetListOutputDto>> GetListAsync(TGetListInput input);

        Task<TGetOutputDto> GetAsync(TKey id);

        Task<TGetOutputDto> CreateAsync(TCreateInput createInput);

        Task<TGetOutputDto> UpdateAsync(TKey id, TUpdateInput updateInput);

        Task DeleteAsync(TKey id);
    }
    
    
}