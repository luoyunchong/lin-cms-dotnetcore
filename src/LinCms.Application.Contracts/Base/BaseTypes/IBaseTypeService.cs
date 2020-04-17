using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LinCms.Application.Contracts.Base.BaseTypes.Dtos;

namespace LinCms.Application.Contracts.Base.BaseTypes
{
    public interface IBaseTypeService
    {
        Task DeleteAsync(int id);

        Task<List<BaseTypeDto>> GetListAsync();

        Task<BaseTypeDto> GetAsync(int id);

        Task CreateAsync(CreateUpdateBaseTypeDto createBaseType);

        Task UpdateAsync(int id, CreateUpdateBaseTypeDto updateBaseType);
    }
}
