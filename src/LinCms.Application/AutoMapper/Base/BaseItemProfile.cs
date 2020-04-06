using AutoMapper;
using LinCms.Application.Contracts.Base.BaseItems;
using LinCms.Application.Contracts.Base.BaseItems.Dtos;
using LinCms.Core.Entities.Base;

namespace LinCms.Application.AutoMapper.Base
{
    public class BaseItemProfile :Profile
    {
        public BaseItemProfile()
        {
            CreateMap<BaseItem, BaseItemDto>();
            CreateMap<CreateUpdateBaseItemDto, BaseItem>();
        }
    }
}
