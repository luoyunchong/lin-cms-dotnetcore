using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinCms.Core.Data;

namespace LinCms.Application.Contracts.Cms.Settings
{
    public interface ISettingService
    {
        PagedResultDto<SettingDto> GetPagedListAsync(PageDto pageDto);

        SettingDto Get(Guid id);

        Task Delete(string name, string providerName, string providerKey);

        Task<List<SettingDto>> GetListAsync(string providerName, string providerKey);

        Task<string> GetOrNullAsync(string name, string providerName, string providerKey);

        Task SetAsync(CreateUpdateSettingDto createSetting);

        Task PostAsync(CreateUpdateSettingDto createSettingDto);

        Task PutAsync(Guid id, CreateUpdateSettingDto updateSettingDto);
    }
}
