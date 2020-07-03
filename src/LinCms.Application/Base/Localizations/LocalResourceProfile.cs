using AutoMapper;
using IGeekFan.Localization.FreeSql.Models;
using LinCms.Application.Contracts.Base.Localizations.Dtos;
using LinCms.Application.Contracts.Blog.Channels.Dtos;

namespace LinCms.Application.Blog.Channels
{
    public class LocalResourceProfile : Profile
    {
        public LocalResourceProfile()
        {
            CreateMap<LocalResource, ResourceDto>();
            CreateMap<ResourceDto, LocalResource>();
        }
    }
}
