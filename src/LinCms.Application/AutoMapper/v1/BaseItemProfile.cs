using AutoMapper;
using LinCms.Application.Contracts.v1.BaseItems;
using LinCms.Core.Entities.Base;

namespace LinCms.Application.AutoMapper.v1
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
