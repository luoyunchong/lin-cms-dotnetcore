using AutoMapper;
using IGeekfan.Localization.FreeSql.Models;

namespace LinCms.Base.Localizations
{
    public class CultureProfile : Profile
    {
        public CultureProfile()
        {
            CreateMap<LocalCulture, CultureDto>();
            CreateMap<CultureDto, LocalCulture>();
        }
    }
}
