using AutoMapper;
using IGeekFan.Localization.FreeSql.Models;
using LinCms.Application.Contracts.Base.Localizations.Dtos;

namespace LinCms.Application.Blog.Channels
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
