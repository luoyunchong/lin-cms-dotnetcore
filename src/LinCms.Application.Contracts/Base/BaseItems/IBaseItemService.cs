using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinCms.Base.BaseItems
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