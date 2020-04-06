using System;
using System.Collections.Generic;
using AutoMapper;
using DotNetCore.CAP;
using FreeSql;
using LinCms.Application.Contracts.Blog.Notifications;
using LinCms.Application.Contracts.Blog.Notifications.Dtos;
using LinCms.Application.Contracts.Blog.UserSubscribes;
using LinCms.Application.Contracts.Blog.UserSubscribes.Dtos;
using LinCms.Application.Contracts.Cms.Users;
using LinCms.Application.Contracts.Cms.Users.Dtos;
using LinCms.Core.Data;
using LinCms.Core.Entities;
using LinCms.Core.Entities.Blog;
using LinCms.Core.Exceptions;
using LinCms.Core.Extensions;
using LinCms.Core.IRepositories;
using LinCms.Core.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.Blog
{
    /// <summary>
    /// 用户订阅
    /// </summary>
    [Route("v1/subscribe")]
    [ApiController]
    [Authorize]
    public class UserSubscribeController : ControllerBase
    {
        private readonly IAuditBaseRepository<UserSubscribe> _userSubscribeRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUser _currentUser;
        private readonly BaseRepository<UserTag> _userTagRepository;
        private readonly ICapPublisher _capBus;
        public UserSubscribeController(IAuditBaseRepository<UserSubscribe> userSubscribeRepository, ICurrentUser currentUser, IUserRepository userRepository, BaseRepository<UserTag> userTagRepository, ICapPublisher capPublisher)
        {
            _userSubscribeRepository = userSubscribeRepository;
            _currentUser = currentUser;
            _userRepository = userRepository;
            _userTagRepository = userTagRepository;
            _capBus = capPublisher;
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

            _capBus.Publish("NotificationController.Post", new CreateNotificationDto()
            {
                NotificationType = NotificationType.UserLikeUser,
                NotificationRespUserId = subscribeUserId,
                UserInfoId = _currentUser.Id ?? 0,
                CreateTime = DateTime.Now,
            });

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
                        Nickname = !r.SubscribeUser.IsDeleted ? r.SubscribeUser.Nickname : "该用户已注销",
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
                        Nickname = !r.LinUser.IsDeleted ? r.LinUser.Nickname:"该用户已注销",
                        Avatar = r.LinUser.Avatar,
                        Username = r.LinUser.Username,
                    },
                    //当前登录的用户是否关注了这个粉丝
                    IsSubscribeed = _userSubscribeRepository.Select.Any(
                        u => u.CreateUserId == _currentUser.Id && u.SubscribeUserId == r.CreateUserId)
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