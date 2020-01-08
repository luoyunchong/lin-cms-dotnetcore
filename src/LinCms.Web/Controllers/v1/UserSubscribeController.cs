﻿using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FreeSql;
using LinCms.Core.Data;
using LinCms.Core.Entities;
using LinCms.Core.Entities.Blog;
using LinCms.Core.Exceptions;
using LinCms.Core.Extensions;
using LinCms.Core.Security;
using LinCms.Infrastructure.Repositories;
using LinCms.Application.Contracts.Cms.Users;
using LinCms.Application.Contracts.v1.UserSubscribes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.v1
{
    /// <summary>
    /// 用户订阅
    /// </summary>
    [Route("v1/subscribe")]
    [ApiController]
    [Authorize]
    public class UserSubscribeController : ControllerBase
    {
        private readonly AuditBaseRepository<UserSubscribe> _userSubscribeRepository;
        private readonly AuditBaseRepository<LinUser> _userRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        private readonly BaseRepository<UserTag> _userTagRepository;
        public UserSubscribeController(AuditBaseRepository<UserSubscribe> userSubscribeRepository, IMapper mapper, ICurrentUser currentUser, AuditBaseRepository<LinUser> userRepository, BaseRepository<UserTag> userTagRepository)
        {
            _userSubscribeRepository = userSubscribeRepository;
            _mapper = mapper;
            _currentUser = currentUser;
            _userRepository = userRepository;
            _userTagRepository = userTagRepository;
        }
        /// <summary>
        /// 判断当前登录的用户是否关注了beSubscribeUserId
        /// </summary>
        /// <param name="subscribeUserId"></param>
        /// <returns></returns>
        [HttpGet("{subscribeUserId}")]
        [AllowAnonymous]
        public bool Get(long subscribeUserId)
        {
            if (_currentUser.Id == null) return false;
            return _userSubscribeRepository.Select.Any(r => r.SubscribeUserId == subscribeUserId && r.CreateUserId == _currentUser.Id);
        }

        /// <summary>
        /// 取消关注用户
        /// </summary>
        /// <param name="subscribeUserId"></param>
        [HttpDelete("{subscribeUserId}")]
        public void Delete(long subscribeUserId)
        {
            bool any = _userSubscribeRepository.Select.Any(r => r.CreateUserId == _currentUser.Id && r.SubscribeUserId == subscribeUserId);
            if (!any)
            {
                throw new LinCmsException("已取消关注");
            }
            _userSubscribeRepository.Delete(r => r.SubscribeUserId == subscribeUserId && r.CreateUserId == _currentUser.Id);
        }

        /// <summary>
        /// 关注用户
        /// </summary>
        /// <param name="subscribeUserId"></param>
        [HttpPost("{subscribeUserId}")]
        public void Post(long subscribeUserId)
        {
            if (subscribeUserId == _currentUser.Id)
            {
                throw new LinCmsException("您无法关注自己");
            }
            LinUser linUser = _userRepository.Select.Where(r => r.Id == subscribeUserId).ToOne();
            if (linUser == null)
            {
                throw new LinCmsException("该用户不存在");
            }

            if (!linUser.IsActive())
            {
                throw new LinCmsException("该用户已被拉黑");
            }

            bool any = _userSubscribeRepository.Select.Any(r =>
                  r.CreateUserId == _currentUser.Id && r.SubscribeUserId == subscribeUserId);
            if (any)
            {
                throw new LinCmsException("您已关注该用户");
            }

            UserSubscribe userSubscribe = new UserSubscribe() { SubscribeUserId = subscribeUserId };
            _userSubscribeRepository.Insert(userSubscribe);
        }

        /// <summary>
        /// 得到某个用户的关注
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public PagedResultDto<UserSubscribeDto> GetUserSubscribeeeList([FromQuery]UserSubscribeSearchDto searchDto)
        {
            var userSubscribes = _userSubscribeRepository.Select.Include(r => r.SubscribeUser)
                .Where(r => r.CreateUserId == searchDto.UserId)
                .OrderByDescending(r => r.CreateTime)
                .ToPager(searchDto, out long count)
                .ToList(r => new UserSubscribeDto
                {
                    CreateUserId = r.CreateUserId,
                    SubscribeUserId = r.SubscribeUserId,
                    Subscribeer = new OpenUserDto()
                    {
                        Id = r.SubscribeUser.Id,
                        Introduction = r.SubscribeUser.Introduction,
                        Nickname = r.SubscribeUser.Nickname,
                        Avatar = r.SubscribeUser.Avatar,
                        Username = r.SubscribeUser.Username,
                    },
                    IsSubscribeed = _userSubscribeRepository.Select.Any(u =>
                        u.CreateUserId == _currentUser.Id && u.SubscribeUserId == r.SubscribeUserId)
                });

            userSubscribes.ForEach(r => { r.Subscribeer.Avatar = _currentUser.GetFileUrl(r.Subscribeer.Avatar); });

            return new PagedResultDto<UserSubscribeDto>(userSubscribes, count);
        }


        /// <summary>
        /// 得到某个用户的粉丝
        /// </summary>
        /// <returns></returns>
        [HttpGet("fans")]
        [AllowAnonymous]
        public PagedResultDto<UserSubscribeDto> GetUserFansList([FromQuery]UserSubscribeSearchDto searchDto)
        {
            List<UserSubscribeDto> userSubscribes = _userSubscribeRepository.Select.Include(r => r.LinUser)
                .Where(r => r.SubscribeUserId == searchDto.UserId)
                .OrderByDescending(r => r.CreateTime)
                .ToPager(searchDto, out long count)
                .ToList(r => new UserSubscribeDto
                {
                    CreateUserId = r.CreateUserId,
                    SubscribeUserId = r.SubscribeUserId,
                    Subscribeer = new OpenUserDto()
                    {
                        Id = r.LinUser.Id,
                        Introduction = r.LinUser.Introduction,
                        Nickname = r.LinUser.Nickname,
                        Avatar = r.LinUser.Avatar,
                        Username = r.LinUser.Username,
                    },
                    //当前登录的用户是否关注了这个粉丝
                    IsSubscribeed = _userSubscribeRepository.Select.Any(u =>
                        u.CreateUserId == _currentUser.Id && u.SubscribeUserId == r.CreateUserId)
                });

            userSubscribes.ForEach(r => { r.Subscribeer.Avatar = _currentUser.GetFileUrl(r.Subscribeer.Avatar); });

            return new PagedResultDto<UserSubscribeDto>(userSubscribes, count);
        }

        /// <summary>
        /// 得到某个用户的关注了、关注者、标签总数
        /// </summary>
        /// <param name="userId"></param>
        [HttpGet("user/{userId}")]
        [AllowAnonymous]
        public SubscribeCountDto GetUserSubscribeInfo(long userId)
        {
            long subscribeCount = _userSubscribeRepository.Select
                .Where(r => r.CreateUserId == userId)
                .Count();

            long fansCount = _userSubscribeRepository.Select
                .Where(r => r.SubscribeUserId == userId)
                .Count();

            long tagCount = _userTagRepository.Select.Include(r => r.Tag)
                .Where(r => r.CreateUserId == userId).Count();

            return new SubscribeCountDto
            {
                SubscribeCount = subscribeCount,
                FansCount = fansCount,
                TagCount = tagCount
            };
        }
    }
}