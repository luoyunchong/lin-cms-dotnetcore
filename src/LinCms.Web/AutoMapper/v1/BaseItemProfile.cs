using AutoMapper;
using LinCms.Web.Models.v1.BaseItems;
using LinCms.Zero.Domain.Base;

namespace LinCms.Web.AutoMapper.v1
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
