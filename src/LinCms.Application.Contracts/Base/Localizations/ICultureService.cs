using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinCms.Base.Localizations;

public interface ICultureService
{
    Task<List<CultureDto>> GetListAsync();

    Task DeleteAsync(long id);

    Task<CultureDto> GetAsync(long id);

    Task<CultureDto> CreateAsync(CultureDto createChannel);

    Task<CultureDto> UpdateAsync(CultureDto updateChannel);
}