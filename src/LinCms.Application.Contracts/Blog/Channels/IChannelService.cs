using System;
using System.Threading.Tasks;
using LinCms.Data;

namespace LinCms.Blog.Channels
{
    public interface IChannelService
    {
        Task DeleteAsync(Guid id);
      
        Task<PagedResultDto<ChannelDto>> GetListAsync(ChannelSearchDto searchDto);

        /// <summary>
        /// 首页减少不必要的字段后，流量字节更少
        /// </summary>
        /// <param name="pageDto"></param>
        /// <returns></returns>
        Task<PagedResultDto<NavChannelListDto>> GetNavListAsync(PageDto pageDto);

        Task<ChannelDto> GetAsync(Guid id);

        Task CreateAsync(CreateUpdateChannelDto createChannel);

        Task UpdateAsync(Guid id, CreateUpdateChannelDto updateChannel);
    }
}
