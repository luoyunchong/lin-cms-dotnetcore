using AutoMapper;
using LinCms.Application.Contracts.Blog.Tags.Dtos;
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.Blog.Tags
{
    public class TagProfile : Profile
    {
        public TagProfile()
        {
            CreateMap<Tag, TagListDto>();
            CreateMap<Tag, TagDto>();
            CreateMap<CreateUpdateTagDto, Tag>();
        }
    }
}
