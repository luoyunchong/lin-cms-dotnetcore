﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LinCms.Application.Contracts.Blog.Tags;
using LinCms.Application.Contracts.Blog.UserSubscribes;
using LinCms.Core.Data;
using LinCms.Core.Entities.Blog;
using LinCms.Core.Exceptions;
using LinCms.Core.Extensions;
using LinCms.Core.Security;
using LinCms.Infrastructure.Repositories;

namespace LinCms.Application.Blog.Tags
{
    public class TagService : ITagService
    {
        private readonly AuditBaseRepository<Tag> _tagRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        private readonly AuditBaseRepository<UserTag> _userTagRepository;
        public TagService(AuditBaseRepository<Tag> tagRepository, IMapper mapper, ICurrentUser currentUser, AuditBaseRepository<UserTag> userTagRepository)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
            _currentUser = currentUser;
            _userTagRepository = userTagRepository;
        }
        /// <summary>
        /// 判断标签是否被关注
        /// </summary>
        /// <param name="tagId"></param>
        /// <returns></returns>
        public bool IsSubscribe(Guid tagId)
        {
            if (_currentUser.Id == null) return false;
            return _userTagRepository.Select.Any(r => r.TagId == tagId && r.CreateUserId == _currentUser.Id);
        }

        public TagDto Get(Guid id)
        {
            Tag tag = _tagRepository.Select.Where(a => a.Id == id).ToOne();
            if (tag == null)
            {
                throw new LinCmsException("不存在此标签");
            }
            TagDto tagDto = _mapper.Map<TagDto>(tag);
            tagDto.IsSubscribe = this.IsSubscribe(id);
            tagDto.ThumbnailDisplay = _currentUser.GetFileUrl(tagDto.Thumbnail);
            return tagDto;
        }

        /// <summary>
        /// 根据状态得到标签列表
        /// </summary>
        /// <param name="searchDto"></param>
        /// <returns></returns>
        public PagedResultDto<TagDto> Get(TagSearchDto searchDto)
        {
            if (searchDto.Sort.IsNullOrEmpty())
            {
                searchDto.Sort = "create_time desc";
            }

            List<TagDto> tags = _tagRepository.Select.IncludeMany(r => r.UserTags, r => r.Where(u => u.CreateUserId == _currentUser.Id))
                .WhereIf(searchDto.TagIds.IsNotNullOrEmpty(), r => searchDto.TagIds.Contains(r.Id))
                .WhereIf(searchDto.TagName.IsNotNullOrEmpty(), r => r.TagName.Contains(searchDto.TagName))
                .WhereIf(searchDto.Status != null, r => r.Status == searchDto.Status)
                .OrderBy(searchDto.Sort)
                .ToPagerList(searchDto, out long totalCount)
                .Select(r =>
                {
                    TagDto tagDto = _mapper.Map<TagDto>(r);
                    tagDto.ThumbnailDisplay = _currentUser.GetFileUrl(tagDto.Thumbnail);
                    tagDto.IsSubscribe = r.UserTags.Any();
                    return tagDto;
                }).ToList();

            return new PagedResultDto<TagDto>(tags, totalCount);
        }

        /// <summary>
        /// 得到某个用户关注的标签
        /// </summary>
        /// <param name="userSubscribeDto"></param>
        /// <returns></returns>
        public PagedResultDto<TagDto> GetSubscribeTags(UserSubscribeSearchDto userSubscribeDto)
        {
            var userTags = _userTagRepository.Select.Include(r => r.Tag)
                .Where(r => r.CreateUserId == userSubscribeDto.UserId)
                .OrderByDescending(r => r.CreateTime)
                .ToPagerList(userSubscribeDto,out long count)
                .Select(r =>
                {
                    TagDto tagDto = _mapper.Map<TagDto>(r.Tag);
                    tagDto.ThumbnailDisplay = _currentUser.GetFileUrl(tagDto.Thumbnail);
                    tagDto.IsSubscribe = true;
                    return tagDto;
                }).ToList();

            return new PagedResultDto<TagDto>(userTags, count);

        }

        public void UpdateArticleCount(Guid? id, int inCreaseCount)
        {
            if (id == null)
            {
                return;
            }

            //防止数量一直减，减到小于0
            if (inCreaseCount < 0)
            {
                Tag tag = _tagRepository.Select.Where(r => r.Id == id).ToOne();
                if (tag.ArticleCount < -inCreaseCount)
                {
                    return;
                }
            }

            _tagRepository.UpdateDiy.Set(r => r.ArticleCount + inCreaseCount)
                .Where(r => r.Id == id)
                .ExecuteAffrows();
        }


        public void UpdateSubscribersCount(Guid? id, int inCreaseCount)
        {
            if (id == null)
            {
                return;
            }

            //防止数量一直减，减到小于0
            if (inCreaseCount < 0)
            {
                Tag tag = _tagRepository.Select.Where(r => r.Id == id).ToOne();
                if (tag.SubscribersCount < -inCreaseCount)
                {
                    return;
                }
            }

            _tagRepository.UpdateDiy.Set(r => r.SubscribersCount + inCreaseCount)
                .Where(r => r.Id == id)
                .ExecuteAffrows();
        }
    }
}