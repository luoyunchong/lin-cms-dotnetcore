using AutoMapper;
using LinCms.Application.Contracts.v1.BaseTypes;
using LinCms.Core.Entities.Base;

namespace LinCms.Application.AutoMapper.v1
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
