using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Aop.Attributes;
using LinCms.Data;
using LinCms.Entities.Blog;
using LinCms.Exceptions;
using LinCms.Extensions;
using LinCms.IRepositories;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Blog.Channels
{
    public class ChannelService : ApplicationService, IChannelService
    {
        private readonly IAuditBaseRepository<Channel, Guid> _channelRepository;
        private readonly IAuditBaseRepository<ChannelTag, Guid> _channelTagRepository;
        private readonly IFileRepository _fileRepository;

        public ChannelService(IAuditBaseRepository<Channel, Guid> channelRepository, IAuditBaseRepository<ChannelTag, Guid> channelTagRepository, IFileRepository fileRepository)
        {
            _channelRepository = channelRepository;
            _channelTagRepository = channelTagRepository;
            _fileRepository = fileRepository;
        }

        public async Task DeleteAsync(Guid id)
        {
            await _channelRepository.DeleteAsync(new Channel { Id = id });
        }

        public async Task<PagedResultDto<ChannelDto>> GetListAsync(ChannelSearchDto searchDto)
        {
            List<ChannelDto> channel = (await _channelRepository.Select
                    .IncludeMany(r => r.Tags, r => r.Where(u => u.Status == true))
                    .WhereIf(searchDto.ChannelName.IsNotNullOrEmpty(), r => r.ChannelName.Contains(searchDto.ChannelName))
                    .OrderByDescending(r => r.SortCode)
                    .OrderBy(r => r.CreateTime)
                    .ToPagerListAsync(searchDto, out long totalCount))
                    .Select(r =>
                    {
                        ChannelDto channelDto = Mapper.Map<ChannelDto>(r);
                        channelDto.ThumbnailDisplay = _fileRepository.GetFileUrl(channelDto.Thumbnail);
                        return channelDto;
                    }).ToList();

            return new PagedResultDto<ChannelDto>(channel, totalCount);
        }

        public async Task<PagedResultDto<NavChannelListDto>> GetNavListAsync(PageDto pageDto)
        {
            List<NavChannelListDto> channel = (await _channelRepository.Select
                    .IncludeMany(r => r.Tags, r => r.Where(u => u.Status == true))
                    .OrderByDescending(r => r.SortCode)
                    .OrderBy(r => r.CreateTime)
                    .ToPagerListAsync(pageDto, out long totalCount))
                    .Select(r => Mapper.Map<NavChannelListDto>(r)).ToList();
            return new PagedResultDto<NavChannelListDto>(channel, totalCount);
        }

        public async Task<ChannelDto> GetAsync(Guid id)
        {
            Channel channel = await _channelRepository.Select
                .IncludeMany(r => r.Tags, r => r.Where(u => u.Status == true))
                .Where(a => a.Id == id)
                .WhereCascade(r => r.IsDeleted == false).ToOneAsync();

            ChannelDto channelDto = Mapper.Map<ChannelDto>(channel);
            channelDto.ThumbnailDisplay = _fileRepository.GetFileUrl(channelDto.Thumbnail);
            return channelDto;
        }

        public async Task CreateAsync([FromBody] CreateUpdateChannelDto createChannel)
        {
            bool exist = await _channelRepository.Select.AnyAsync(r => r.ChannelName == createChannel.ChannelName && r.ChannelCode == createChannel.ChannelCode);
            if (exist)
            {
                throw new LinCmsException($"技术频道[{createChannel.ChannelName}]已存在");
            }

            Channel channel = Mapper.Map<Channel>(createChannel);

            channel.Tags = new List<Tag>();
            createChannel.TagIds?.ForEach(r =>
            {
                channel.Tags.Add(new Tag()
                {
                    Id = r
                });
            });

            await _channelRepository.InsertAsync(channel);
        }

        [Transactional]
        public async Task UpdateAsync(Guid id, CreateUpdateChannelDto updateChannel)
        {
            Channel channel = await _channelRepository.Select.Where(r => r.Id == id).ToOneAsync();
            if (channel == null)
            {
                throw new LinCmsException("该数据不存在");
            }

            bool exist = await _channelRepository.Select.AnyAsync(r => r.ChannelName == updateChannel.ChannelName && r.Id != id && r.ChannelCode == updateChannel.ChannelCode);
            if (exist)
            {
                throw new LinCmsException($"技术频道[{updateChannel.ChannelName}]已存在");
            }

            Mapper.Map(updateChannel, channel);
            await _channelRepository.UpdateAsync(channel);
            await _channelTagRepository.DeleteAsync(r => r.ChannelId == id);

            var channelTagLists = new List<ChannelTag>();
            updateChannel.TagIds?.ForEach(r => { channelTagLists.Add(new ChannelTag(id, r)); });
            await _channelTagRepository.InsertAsync(channelTagLists);

        }
    }
}
