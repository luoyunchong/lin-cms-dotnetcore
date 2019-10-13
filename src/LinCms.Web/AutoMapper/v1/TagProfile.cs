using AutoMapper;
using LinCms.Web.Models.v1.BaseItems;
using LinCms.Web.Models.v1.Tags;
using LinCms.Zero.Domain.Base;
using LinCms.Zero.Domain.Blog;

namespace LinCms.Web.AutoMapper.v1
{
    public class TagProfile : Profile
    {
        public TagProfile()
        {
            CreateMap<Tag, TagDto>();
            CreateMap<CreateUpdateTagDto, Tag>();
        }
    }
}
