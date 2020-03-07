using AutoMapper;
using LinCms.Application.Contracts.Cms.Settings;
using LinCms.Core.Entities.Settings;

namespace LinCms.Application.AutoMapper.Cms
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
