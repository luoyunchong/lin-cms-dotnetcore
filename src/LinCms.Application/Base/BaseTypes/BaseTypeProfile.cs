using AutoMapper;
using LinCms.Entities.Base;

namespace LinCms.Base.BaseTypes;

public class BaseTypeProfile : Profile
{
    public BaseTypeProfile()
    {
        CreateMap<BaseType, BaseTypeDto>();
        CreateMap<CreateUpdateBaseTypeDto, BaseType>();
    }
}