using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LinCms.Application.Contracts.Blog.Channels;
using LinCms.Application.Contracts.Blog.Channels.Dtos;
using LinCms.Core.Data;
using LinCms.Core.Entities.Blog;
using LinCms.Core.Exceptions;
using LinCms.Core.Extensions;
using LinCms.Core.IRepositories;
using LinCms.Core.Security;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Application.Blog.Channels
{
    public class ChannelService : IChannelService
    {
        private readonly IAuditBaseRepository<Channel> _channelRepository;
        private readonly IAuditBaseRepository<ChannelTag> _channelTagRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public ChannelService(IMapper mapper, IAuditBaseRepository<Channel> channelRepository, ICurrentUser currentUser, IAuditBaseRepository<ChannelTag> channelTagRepository)
        {
            _mapper = mapper;
            _channelRepository = channelRepository;
            _currentUser = currentUser;
            _channelTagRepository = channelTagRepository;
        }

        public async Task DeleteAsync(Guid id)
        {
            await _channelRepository.DeleteAsync(new Channel { Id = id });
        }

        public async Task<PagedResultDto<ChannelDto>> GetListAsync(PageDto pageDto)
        {
            List<ChannelDto> channel = (await _channelRepository.Select
                    .IncludeMany(r => r.Tags, r => r.Where(u => u.Status == true))
                    .OrderByDescending(r => r.SortCode)
                    .OrderBy(r => r.CreateTime)
                    .ToPagerListAsync(pageDto, out long totalCount))
                    .Select(r =>
                    {
                        ChannelDto channelDto = _mapper.Map<ChannelDto>(r);
                        channelDto.ThumbnailDisplay = _currentUser.GetFileUrl(channelDto.Thumbnail);
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
                    .Select(r => _mapper.Map<NavChannelListDto>(r)).ToList();
            return new PagedResultDto<NavChannelListDto>(channel, totalCount);
        }

        public async Task<ChannelDto> GetAsync(Guid id)
        {
            Channel channel = await _channelRepository.Select
                .IncludeMany(r => r.Tags, r => r.Where(u => u.Status == true))
                .Where(a => a.Id == id)
                .WhereCascade(r => r.IsDeleted == false).ToOneAsync();

            ChannelDto channelDto = _mapper.Map<ChannelDto>(channel);
            channelDto.ThumbnailDisplay = _currentUser.GetFileUrl(channelDto.Thumbnail);
            return channelDto;
        }

        public async Task CreateAsync([FromBody] CreateUpdateChannelDto createChannel)
        {
            bool exist = await _channelRepository.Select.AnyAsync(r => r.ChannelName == createChannel.ChannelName && r.ChannelCode == createChannel.ChannelCode);
            if (exist)
            {
                throw new LinCmsException($"技术频道[{createChannel.ChannelName}]已存在");
            }

            Channel channel = _mapper.Map<Channel>(createChannel);

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

        public async Task UpdateAsync(Guid id, CreateUpdateChannelDto updateChannel)
        {
            Channel channel = await _channelRepository.Select.Where(r => r.Id == id).ToOneAsync();
            if (channel == null)
            {
                throw new LinCmsException("该数据不存在");
            }

            bool exist = _channelRepository.Select.Any(r => r.ChannelName == updateChannel.ChannelName && r.Id != id && r.ChannelCode == updateChannel.ChannelCode);
            if (exist)
            {
                throw new LinCmsException($"技术频道[{updateChannel.ChannelName}]已存在");
            }

            _mapper.Map(updateChannel, channel);

            var channelTagLists = new List<ChannelTag>();
            updateChannel.TagIds?.ForEach(r => { channelTagLists.Add(new ChannelTag(id, r)); });

            await _channelTagRepository.DeleteAsync(r => r.ChannelId == id);
            await _channelRepository.UpdateAsync(channel);
            await _channelTagRepository.InsertAsync(channelTagLists);

        }
    }
}
