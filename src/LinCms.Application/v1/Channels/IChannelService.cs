using System;
using System.Collections.Generic;
using System.Text;
using LinCms.Application.Contracts.v1.Channels;
using LinCms.Core.Data;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Application.v1.Channels
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
