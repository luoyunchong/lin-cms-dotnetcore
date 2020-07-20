using AutoMapper;
using IGeekfan.Localization.FreeSql.Models;

namespace LinCms.Base.Localizations
{
    public class ResourceProfile : Profile
    {
        public ResourceProfile()
        {
            CreateMap<LocalResource, ResourceDto>();
            CreateMap<ResourceDto, LocalResource>();
        }
    }
}
