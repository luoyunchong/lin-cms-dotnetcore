using AutoMapper;
using LinCms.Entities.Settings;

namespace LinCms.Cms.Settings
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
