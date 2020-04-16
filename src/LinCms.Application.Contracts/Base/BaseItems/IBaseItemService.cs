using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LinCms.Application.Contracts.Base.BaseItems.Dtos;
using LinCms.Application.Contracts.Base.BaseTypes.Dtos;

namespace LinCms.Application.Contracts.Base.BaseItems
{
    public interface IBaseItemService
    {
        Task DeleteAsync(int id);

        Task<List<BaseItemDto>> GetListAsync(string typeCode);

        Task<BaseItemDto> GetAsync(int id);

        Task CreateAsync(CreateUpdateBaseItemDto createBaseItem);

        Task UpdateAsync(int id, CreateUpdateBaseItemDto updateBaseItem);
    }
}