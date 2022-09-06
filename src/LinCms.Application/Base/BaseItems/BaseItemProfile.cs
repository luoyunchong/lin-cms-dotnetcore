using AutoMapper;
using LinCms.Entities.Base;

namespace LinCms.Base.BaseItems;

public class BaseItemProfile : Profile
{
    public BaseItemProfile()
    {
        CreateMap<BaseItem, BaseItemDto>();
        CreateMap<CreateUpdateBaseItemDto, BaseItem>();
    }
}