using AutoMapper;
using LinCms.Application.Contracts.Base.BaseTypes;
using LinCms.Application.Contracts.Base.BaseTypes.Dtos;
using LinCms.Core.Entities.Base;

namespace LinCms.Application.AutoMapper.Base
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
