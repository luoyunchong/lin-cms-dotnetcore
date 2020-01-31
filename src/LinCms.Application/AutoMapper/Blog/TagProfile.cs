using AutoMapper;
using LinCms.Application.Contracts.Blog.Tags;
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.AutoMapper.Blog
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
