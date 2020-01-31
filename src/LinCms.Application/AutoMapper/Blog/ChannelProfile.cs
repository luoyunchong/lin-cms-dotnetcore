using AutoMapper;
using LinCms.Application.Contracts.Blog.Channels;
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.AutoMapper.Blog
{
    public class ChannelProfile : Profile
    {
        public ChannelProfile()
        {
            CreateMap<CreateUpdateChannelDto, Channel>();
            CreateMap<Channel, ChannelDto>();
        }
    }
}
