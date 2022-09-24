using AutoMapper;
using LinCms.Entities.Blog;

namespace LinCms.Blog.Channels;

public class ChannelProfile : Profile
{
    public ChannelProfile()
    {
        CreateMap<CreateUpdateChannelDto, Channel>();
        CreateMap<Channel, ChannelDto>();
        CreateMap<Channel, NavChannelListDto>();
    }
}