using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LinCms.Application.Contracts.v1.Channels;
using LinCms.Application.Contracts.v1.Tags;
using LinCms.Core.Data;
using LinCms.Core.Entities.Blog;
using LinCms.Core.Exceptions;
using LinCms.Core.Extensions;
using LinCms.Core.Security;
using LinCms.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Application.v1.Channels
{
    public class ChannelService : IChannelService
    {
        private readonly AuditBaseRepository<Channel> _channelRepository;
        private readonly AuditBaseRepository<ChannelTag> _channelTagRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public ChannelService(IMapper mapper, AuditBaseRepository<Channel> channelRepository, ICurrentUser currentUser, AuditBaseRepository<ChannelTag> channelTagRepository)
        {
            _mapper = mapper;
            _channelRepository = channelRepository;
            _currentUser = currentUser;
            _channelTagRepository = channelTagRepository;
        }

        public void Delete(Guid id)
        {
            _channelRepository.Delete(new Channel { Id = id });
        }

        public PagedResultDto<ChannelDto> Get(PageDto pageDto)
        {
            List<ChannelDto> channel = _channelRepository.Select
                .IncludeMany(r => r.Tags, r => r.Where(u => u.Status == true))
                .OrderByDescending(r => r.SortCode)
                .OrderBy(r => r.CreateTime)
                .ToPagerList(pageDto, out long totalCount)
                .Select(r =>
                {
                    ChannelDto channelDto = _mapper.Map<ChannelDto>(r);
                    channelDto.ThumbnailDisplay = _currentUser.GetFileUrl(channelDto.Thumbnail);

                    channelDto.Tags = r.Tags.Select(u => new TagDto()
                    {
                        TagName = u.TagName,
                        Id = u.Id,
                    }).ToList();

                    return channelDto;
                }).ToList();

            return new PagedResultDto<ChannelDto>(channel, totalCount);
        }

        public ChannelDto Get(Guid id)
        {
            Channel channel = _channelRepository.Select
                .IncludeMany(r => r.Tags, r => r.Where(u => u.Status == true))
                .Where(a => a.Id == id)
                .WhereCascade(r => r.IsDeleted == false).ToOne();

            ChannelDto channelDto = _mapper.Map<ChannelDto>(channel);
            channelDto.ThumbnailDisplay = _currentUser.GetFileUrl(channelDto.Thumbnail);
            return channelDto;
        }

        public void Post([FromBody] CreateUpdateChannelDto createChannel)
        {
            bool exist = _channelRepository.Select.Any(r => r.ChannelName == createChannel.ChannelName && r.ChannelCode == createChannel.ChannelCode);
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

            _channelRepository.Insert(channel);
        }

        public void Put(Guid id, CreateUpdateChannelDto updateChannel)
        {
            Channel channel = _channelRepository.Select.Where(r => r.Id == id).ToOne();
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

            _channelTagRepository.Delete(r => r.ChannelId == id);
            _channelRepository.Update(channel);
            _channelTagRepository.Insert(channelTagLists);

        }
    }
}
