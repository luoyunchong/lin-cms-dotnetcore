using System;
using System.Threading.Tasks;
using LinCms.Application.Blog.Channels;
using LinCms.Application.Contracts.Blog.Channels;
using LinCms.Application.Contracts.Blog.Channels.Dtos;
using LinCms.Core.Aop;
using LinCms.Core.Data;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.Blog
{
    /// <summary>
    /// 技术频道
    /// </summary>
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
        public async Task<UnifyResponseDto> DeleteAsync(Guid id)
        {
            await _channelService.DeleteAsync(id);
            return UnifyResponseDto.Success();
        }

        [LinCmsAuthorize("技术频道列表", "技术频道")]
        [HttpGet]
        public Task<PagedResultDto<ChannelDto>> GetListAsync([FromQuery]PageDto pageDto)
        {
            return _channelService.GetListAsync(pageDto);
        }

        [HttpGet("nav")]
        public Task<PagedResultDto<NavChannelListDto>> GetNavListAsync([FromQuery]PageDto pageDto)
        {
            return _channelService.GetNavListAsync(pageDto);
        }

        [HttpGet("{id}")]
        public Task<ChannelDto> GetAsync(Guid id)
        {
            return _channelService.GetAsync(id);
        }

        [LinCmsAuthorize("新增技术频道", "技术频道")]
        [HttpPost]
        public UnifyResponseDto CreateAsync([FromBody] CreateUpdateChannelDto createChannel)
        {
            _channelService.CreateAsync(createChannel);
            return UnifyResponseDto.Success("新建技术频道成功");
        }

        [LinCmsAuthorize("修改技术频道", "技术频道")]
        [HttpPut("{id}")]
        public async Task<UnifyResponseDto> UpdateAsync(Guid id, [FromBody] CreateUpdateChannelDto updateChannel)
        {
            await _channelService.UpdateAsync(id, updateChannel);
            return UnifyResponseDto.Success("更新技术频道成功");
        }
    }
}
