using AutoMapper;
using LinCms.Application.Contracts.Base.BaseTypes.Dtos;
using LinCms.Core.Entities.Base;

namespace LinCms.Application.Base.BaseTypes
{
    public class BaseTypeProfile:Profile
    {
        public BaseTypeProfile()
        {
            CreateMap<BaseType, BaseTypeDto>();
            CreateMap<CreateUpdateBaseTypeDto, BaseType>();
        }
    }
}
