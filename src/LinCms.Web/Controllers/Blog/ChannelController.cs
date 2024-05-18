using System;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dto;
using LinCms.Aop.Filter;
using LinCms.Blog.Channels;
using LinCms.Data;

using Microsoft.AspNetCore.Mvc;

namespace LinCms.Controllers.Blog;

/// <summary>
/// 技术频道
/// </summary>
[ApiExplorerSettings(GroupName = "blog")]
[Area("blog")]
[Route("api/blog/channels")]
[ApiController]
public class ChannelController(IChannelService channelService) : ControllerBase
{
    [LinCmsAuthorize("删除技术频道", "技术频道")]
    [HttpDelete("{id}")]
    public async Task<UnifyResponseDto> DeleteAsync(Guid id)
    {
        await channelService.DeleteAsync(id);
        return UnifyResponseDto.Success();
    }

    [LinCmsAuthorize("技术频道列表", "技术频道")]
    [HttpGet]
    public Task<PagedResultDto<ChannelDto>> GetListAsync([FromQuery] ChannelSearchDto searchDto)
    {
        return channelService.GetListAsync(searchDto);
    }

    /// <summary>
    /// 首页显示频道及对应的标签列
    /// </summary>
    /// <param name="pageDto"></param>
    /// <returns></returns>
    [HttpGet("nav")]
    public async Task<PagedResultDto<NavChannelListDto>> GetNavListAsync([FromQuery] PageDto pageDto)
    {
        return await channelService.GetNavListAsync(pageDto);
    }

    [HttpGet("{id}")]
    public Task<ChannelDto> GetAsync(Guid id)
    {
        return channelService.GetAsync(id);
    }

    [LinCmsAuthorize("新增技术频道", "技术频道")]
    [HttpPost]
    public async Task<UnifyResponseDto> CreateAsync([FromBody] CreateUpdateChannelDto createChannel)
    {
        await channelService.CreateAsync(createChannel);
        return UnifyResponseDto.Success("新建技术频道成功");
    }

    [LinCmsAuthorize("修改技术频道", "技术频道")]
    [HttpPut("{id}")]
    public async Task<UnifyResponseDto> UpdateAsync(Guid id, [FromBody] CreateUpdateChannelDto updateChannel)
    {
        await channelService.UpdateAsync(id, updateChannel);
        return UnifyResponseDto.Success("更新技术频道成功");
    }
}