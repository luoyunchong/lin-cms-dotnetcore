using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinCms.Base.BaseTypes
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
