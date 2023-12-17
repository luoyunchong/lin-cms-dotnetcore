using System;
using System.Threading.Tasks;
using IGeekFan.FreeKit.Extras.Dto;
using LinCms.Data;

namespace LinCms.Blog.Channels;

/// <summary>
/// 技术频道
/// </summary>
public interface IChannelService
{
    #region CRUD
    Task<PagedResultDto<ChannelDto>> GetListAsync(ChannelSearchDto searchDto);

    Task<ChannelDto> GetAsync(Guid id);

    Task CreateAsync(CreateUpdateChannelDto createChannel);

    Task UpdateAsync(Guid id, CreateUpdateChannelDto updateChannel);
    Task DeleteAsync(Guid id);
    #endregion

    /// <summary>
    /// 首页减少不必要的字段后，流量字节更少
    /// </summary>
    /// <param name="pageDto"></param>
    /// <returns></returns>
    Task<PagedResultDto<NavChannelListDto>> GetNavListAsync(PageDto pageDto);
}