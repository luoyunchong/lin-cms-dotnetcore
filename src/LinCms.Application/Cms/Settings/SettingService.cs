using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Application.Contracts.Cms.Settings;
using LinCms.Core.Data;
using LinCms.Core.Entities.Settings;
using LinCms.Core.Exceptions;
using LinCms.Core.Extensions;
using LinCms.Core.IRepositories;

namespace LinCms.Application.Cms.Settings
{
    public class SettingService : ISettingService
    {
        private readonly IMapper _mapper;
        private readonly ISettingRepository _settingRepository;

        public SettingService(ISettingRepository settingRepository, IMapper mapper)
        {
            _settingRepository = settingRepository;
            _mapper = mapper;
        }

        public async Task Delete(string name, string providerName, string providerKey)
        {
            LinSetting setting = await _settingRepository.FindAsync(name, providerName, providerKey); ;
            if (setting != null)
            {
                await _settingRepository.DeleteAsync(setting.Id);
            }
        }

        public async Task<List<SettingDto>> GetListAsync(string providerName, string providerKey)
        {
            var list = await _settingRepository.GetListAsync(providerName, providerName);

            return _mapper.Map<List<SettingDto>>(list);
        }

        public async Task<string> GetOrNullAsync(string name, string providerName, string providerKey)
        {
            LinSetting settings = await _settingRepository.FindAsync(name, providerName, providerKey);
            return settings?.Value;
        }

        public async Task SetAsync(CreateUpdateSettingDto createSetting)
        {
            LinSetting setting = await _settingRepository.FindAsync(createSetting.Name, createSetting.ProviderName, createSetting.ProviderKey); ;
            if (setting == null)
            {
                _settingRepository.Insert(_mapper.Map<LinSetting>(createSetting));
            }
            else
            {
                setting.Value = createSetting.Value;
                _settingRepository.Update(setting);
            }
        }

        public SettingDto Get(Guid id)
        {
            return _mapper.Map<SettingDto>(_settingRepository.Get(id));
        }
        public PagedResultDto<SettingDto> GetPagedListAsync(PageDto pageDto)
        {
            List<SettingDto> list = _settingRepository.Select.ToPagerList(pageDto, out long totalCount)
                .Select(r => _mapper.Map<SettingDto>(r)).ToList();

            return new PagedResultDto<SettingDto>(list, totalCount);
        }

        public async Task PostAsync(CreateUpdateSettingDto createSettingDto)
        {
            LinSetting setting = await _settingRepository.FindAsync(createSettingDto.Name, createSettingDto.ProviderName, createSettingDto.ProviderKey); ;
            if (setting != null)
            {
                throw new LinCmsException("该配置已存在");
            }
            await _settingRepository.InsertAsync(_mapper.Map<LinSetting>(createSettingDto));
        }

        public async Task PutAsync(Guid id, CreateUpdateSettingDto updateSettingDto)
        {
            LinSetting setting = await _settingRepository.Select.Where(r => r.Id == id).ToOneAsync();
            if (setting == null)
            {
                throw new LinCmsException("该数据不存在");
            }

            LinSetting settingExist = await _settingRepository.Select
                .Where(s => s.Name == updateSettingDto.Name && s.ProviderName == updateSettingDto.ProviderName && s.ProviderKey == updateSettingDto.ProviderKey && s.Id != id)
                .FirstAsync();

            if (settingExist != null)
            {
                throw new LinCmsException("该配置已存在");
            }

            await _settingRepository.UpdateAsync(_mapper.Map(updateSettingDto, setting));
        }
    }
}
