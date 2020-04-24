using AutoMapper;
using LinCms.Application.Contracts.Blog.Channels.Dtos;
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.Blog.Channels
{
    public class ChannelProfile : Profile
    {
        public ChannelProfile()
        {
            CreateMap<CreateUpdateChannelDto, Channel>();
            CreateMap<Channel, ChannelDto>();
            CreateMap<Channel, NavChannelListDto>();
        }
    }
}
