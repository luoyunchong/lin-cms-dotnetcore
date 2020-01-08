using AutoMapper;
using LinCms.Application.Contracts.v1.Tags;
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.AutoMapper.v1
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
