using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dto;
using LinCms.Data;
using LinCms.Entities.Settings;
using LinCms.Exceptions;
using LinCms.Extensions;
using LinCms.IRepositories;

namespace LinCms.Cms.Settings;

public class SettingService(ISettingRepository settingRepository) : ApplicationService, ISettingService
{
    public async Task Delete(string name, string providerName, string providerKey)
    {
        LinSetting setting = await settingRepository.FindAsync(name, providerName, providerKey);
        ;
        if (setting != null)
        {
            await settingRepository.DeleteAsync(setting.Id);
        }
    }

    public async Task<List<SettingDto>> GetListAsync(string providerName, string providerKey)
    {
        var list = await settingRepository.GetListAsync(providerName, providerName);

        return Mapper.Map<List<SettingDto>>(list);
    }

    public async Task<string> GetOrNullAsync(string name, string providerName, string providerKey)
    {
        LinSetting settings = await settingRepository.FindAsync(name, providerName, providerKey);
        return settings?.Value;
    }

    public async Task SetAsync(CreateUpdateSettingDto createSetting)
    {
        LinSetting setting = await settingRepository.FindAsync(createSetting.Name, createSetting.ProviderName,
            createSetting.ProviderKey);

        if (setting == null)
        {
            await settingRepository.InsertAsync(Mapper.Map<LinSetting>(createSetting));
        }
        else
        {
            setting.Value = createSetting.Value;
            await settingRepository.UpdateAsync(setting);
        }
    }

    public async Task<SettingDto> GetAsync(Guid id)
    {
        return Mapper.Map<SettingDto>(await settingRepository.GetAsync(id));
    }

    public async Task<PagedResultDto<SettingDto>> GetPagedListAsync(PageDto pageDto)
    {
        List<SettingDto> list =
            (await settingRepository.Select.OrderByDescending(r => r.CreateTime)
                .ToPagerListAsync(pageDto, out long totalCount))
            .Select(r => Mapper.Map<SettingDto>(r)).ToList();

        return new PagedResultDto<SettingDto>(list, totalCount);
    }

    public async Task CreateAsync(CreateUpdateSettingDto createSettingDto)
    {
        LinSetting setting = await settingRepository.FindAsync(createSettingDto.Name,
            createSettingDto.ProviderName, createSettingDto.ProviderKey);
      
        if (setting != null)
        {
            throw new LinCmsException("该配置已存在");
        }

        await settingRepository.InsertAsync(Mapper.Map<LinSetting>(createSettingDto));
    }

    public async Task UpdateAsync(Guid id, CreateUpdateSettingDto updateSettingDto)
    {
        LinSetting setting = await settingRepository.Select.Where(r => r.Id == id).ToOneAsync();
        if (setting == null)
        {
            throw new LinCmsException("该数据不存在");
        }

        LinSetting settingExist = await settingRepository.Select
            .Where(s => s.Name == updateSettingDto.Name && s.ProviderName == updateSettingDto.ProviderName &&
                        s.ProviderKey == updateSettingDto.ProviderKey && s.Id != id)
            .FirstAsync();

        if (settingExist != null)
        {
            throw new LinCmsException("该配置已存在");
        }

        await settingRepository.UpdateAsync(Mapper.Map(updateSettingDto, setting));
    }
}