using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IGeekFan.Localization.FreeSql.Models;
using LinCms.Application.Contracts.Base.Localizations.Dtos;
using LinCms.Core.Data;

namespace LinCms.Application.Contracts.Base.Localizations
{
    public interface ILocalResourceService
    {
        Task<PagedResultDto<ResourceDto>> GetListAsync(ResourceSearchDto searchDto);

        Task DeleteAsync(long id);

        Task<ResourceDto> GetAsync(long id);

        Task CreateAsync(ResourceDto createChannel);

        Task UpdateAsync(ResourceDto updateChannel);
    }
}
