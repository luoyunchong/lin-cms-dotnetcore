using AutoMapper;
using LinCms.Application.Contracts.Cms.Settings;
using LinCms.Application.Contracts.Cms.Settings.Dtos;
using LinCms.Core.Entities.Settings;

namespace LinCms.Application.Cms.Settings
{
    public class SettingProfile : Profile
    {
        public SettingProfile()
        {
            CreateMap<CreateUpdateSettingDto, LinSetting>();
            CreateMap<LinSetting, SettingDto>();
        }
    }
}
