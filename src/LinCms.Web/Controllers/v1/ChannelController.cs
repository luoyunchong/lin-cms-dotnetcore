using System;
using LinCms.Application.Contracts.v1.Channels;
using LinCms.Application.v1.Channels;
using LinCms.Core.Aop;
using LinCms.Core.Data;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.v1
{
    [Route("v1/channel")]
    [ApiController]
    public class ChannelController : ControllerBase
    {
        private readonly IChannelService _channelService;
        public ChannelController(IChannelService channelService)
        {
            _channelService = channelService;
        }


        [LinCmsAuthorize("删除技术频道", "技术频道")]
        [HttpDelete("{id}")]
        public ResultDto Delete(Guid id)
        {
            _channelService.Delete(id);
            return ResultDto.Success();
        }

        [HttpGet]
        public PagedResultDto<ChannelDto> Get([FromQuery]PageDto pageDto)
        {
            return _channelService.Get(pageDto);
        }

        [HttpGet("{id}")]
        public ChannelDto Get(Guid id)
        {
            return _channelService.Get(id);
        }

        [LinCmsAuthorize("新增技术频道", "技术频道")]
        [HttpPost]
        public ResultDto Post([FromBody] CreateUpdateChannelDto createChannel)
        {
            _channelService.Post(createChannel);
            return ResultDto.Success("新建技术频道成功");
        }

        [LinCmsAuthorize("修改技术频道", "技术频道")]
        [HttpPut("{id}")]
        public ResultDto Put(Guid id, [FromBody] CreateUpdateChannelDto updateChannel)
        {
            _channelService.Put(id, updateChannel);

            return ResultDto.Success("更新技术频道成功");
        }
    }
}
