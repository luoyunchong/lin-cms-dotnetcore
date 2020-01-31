using System;
using LinCms.Application.Contracts.Blog.Channels;
using LinCms.Core.Data;

namespace LinCms.Application.Blog.Channels
{
    public interface IChannelService
    {
        void Delete(Guid id);
        PagedResultDto<ChannelDto> Get(PageDto pageDto);

        ChannelDto Get(Guid id);

        void Post(CreateUpdateChannelDto createChannel);

        void Put(Guid id, CreateUpdateChannelDto updateChannel);
    }
}
