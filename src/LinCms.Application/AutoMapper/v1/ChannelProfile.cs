using System;
using AutoMapper;
using LinCms.Application.Contracts.v1.Channels;
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.AutoMapper.v1
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
