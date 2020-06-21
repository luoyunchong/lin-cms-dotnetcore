using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    [Area ("blog")]
    [Route ("api/blog/subscribe")]
    [ApiController]
    [Authorize]
    public class UserSubscribeController : ControllerBase
    {
        private readonly IAuditBaseRepository<UserSubscribe> _userSubscribeRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IAuditBaseRepository<UserTag> _userTagRepository;
        private readonly ICapPublisher _capBus;
        private readonly IFileRepository _fileRepository;
        private readonly UnitOfWorkManager _unitOfWorkManager;

        public UserSubscribeController (IAuditBaseRepository<UserSubscribe> userSubscribeRepository,
            ICurrentUser currentUser, IUserRepository userRepository, IAuditBaseRepository<UserTag> userTagRepository,
            ICapPublisher capPublisher, IFileRepository fileRepository, UnitOfWorkManager unitOfWorkManager)
        {
            _userSubscribeRepository = userSubscribeRepository;
            _currentUser = currentUser;
            _userRepository = userRepository;
            _userTagRepository = userTagRepository;
            _capBus = capPublisher;
            _fileRepository = fileRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        /// <summary>
        /// 判断当前登录的用户是否关注了beSubscribeUserId
        /// </summary>
        /// <param name="subscribeUserId"></param>
        /// <returns></returns>
        [HttpGet ("{subscribeUserId}")]
        [AllowAnonymous]
        public bool Get (long subscribeUserId)
        {
            if (_currentUser.Id == null) return false;
            return _userSubscribeRepository.Select.Any (r =>
                r.SubscribeUserId == subscribeUserId && r.CreateUserId == _currentUser.Id);
        }

        /// <summary>
        /// 取消关注用户
        /// </summary>
        /// <param name="subscribeUserId"></param>
        [HttpDelete ("{subscribeUserId}")]
        public async Task DeleteAsync (long subscribeUserId)
        {
            bool any = await _userSubscribeRepository.Select.AnyAsync (r =>
                r.CreateUserId == _currentUser.Id && r.SubscribeUserId == subscribeUserId);
            if (!any)
            {
                throw new LinCmsException ("已取消关注");
            }

            using IUnitOfWork unitOfWork = _unitOfWorkManager.Begin ();
            using ICapTransaction capTransaction = unitOfWork.BeginTransaction (_capBus, false);

            await _userSubscribeRepository.DeleteAsync (r =>
                r.SubscribeUserId == subscribeUserId && r.CreateUserId == _currentUser.Id);

            await _capBus.PublishAsync ("NotificationController.Post", new CreateNotificationDto ()
            {
                NotificationType = NotificationType.UserLikeUser,
                    NotificationRespUserId = subscribeUserId,
                    UserInfoId = _currentUser.Id ?? 0,
                    CreateTime = DateTime.Now,
                    IsCancel = true
            });

            await capTransaction.CommitAsync ();
        }

        /// <summary>
        /// 关注用户
        /// </summary>
        /// <param name="subscribeUserId"></param>
        [HttpPost ("{subscribeUserId}")]
        public async Task Post (long subscribeUserId)
        {
            if (subscribeUserId == _currentUser.Id)
            {
                throw new LinCmsException ("您无法关注自己");
            }

            LinUser linUser = _userRepository.Select.Where (r => r.Id == subscribeUserId).ToOne ();
            if (linUser == null)
            {
                throw new LinCmsException ("该用户不存在");
            }

            if (!linUser.IsActive ())
            {
                throw new LinCmsException ("该用户已被拉黑");
            }

            bool any = _userSubscribeRepository.Select.Any (r =>
                r.CreateUserId == _currentUser.Id && r.SubscribeUserId == subscribeUserId);
            if (any)
            {
                throw new LinCmsException ("您已关注该用户");
            }

            using (IUnitOfWork unitOfWork = _unitOfWorkManager.Begin ())
            {
                using ICapTransaction capTransaction = unitOfWork.BeginTransaction (_capBus, false);

                UserSubscribe userSubscribe = new UserSubscribe () { SubscribeUserId = subscribeUserId };
                await _userSubscribeRepository.InsertAsync (userSubscribe);

                await _capBus.PublishAsync ("NotificationController.Post", new CreateNotificationDto ()
                {
                    NotificationType = NotificationType.UserLikeUser,
                        NotificationRespUserId = subscribeUserId,
                        UserInfoId = _currentUser.Id ?? 0,
                        CreateTime = DateTime.Now,
                });

                await capTransaction.CommitAsync ();
            }
        }

        /// <summary>
        /// 得到某个用户的关注
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public PagedResultDto<UserSubscribeDto> GetUserSubscribeeeList ([FromQuery] UserSubscribeSearchDto searchDto)
        {
            List<UserSubscribeDto> userSubscribes = _userSubscribeRepository.Select.Include (r => r.SubscribeUser)
                .Where (r => r.CreateUserId == searchDto.UserId)
                .OrderByDescending (r => r.CreateTime)
                .ToPager (searchDto, out long count)
                .ToList (r => new UserSubscribeDto
                {
                    CreateUserId = r.CreateUserId,
                        SubscribeUserId = r.SubscribeUserId,
                        Subscribeer = new OpenUserDto ()
                        {
                            Id = r.SubscribeUser.Id,
                                Introduction = r.SubscribeUser.Introduction,
                                Nickname = !r.SubscribeUser.IsDeleted ? r.SubscribeUser.Nickname : "该用户已注销",
                                Avatar = r.SubscribeUser.Avatar,
                                Username = r.SubscribeUser.Username,
                        },
                        IsSubscribeed = _userSubscribeRepository.Select.Any (r => r.CreateUserId == _currentUser.Id && r.SubscribeUserId == r.SubscribeUserId)
                });

            userSubscribes.ForEach (r => { r.Subscribeer.Avatar = _fileRepository.GetFileUrl (r.Subscribeer.Avatar); });

            return new PagedResultDto<UserSubscribeDto> (userSubscribes, count);
        }

        /// <summary>
        /// 得到某个用户的粉丝
        /// </summary>
        /// <returns></returns>
        [HttpGet ("fans")]
        [AllowAnonymous]
        public PagedResultDto<UserSubscribeDto> GetUserFansList ([FromQuery] UserSubscribeSearchDto searchDto)
        {
            List<UserSubscribeDto> userSubscribes = _userSubscribeRepository.Select.Include (r => r.LinUser)
                .Where (r => r.SubscribeUserId == searchDto.UserId)
                .OrderByDescending (r => r.CreateTime)
                .ToPager (searchDto, out long count)
                .ToList (r => new UserSubscribeDto
                {
                    CreateUserId = r.CreateUserId,
                        SubscribeUserId = r.SubscribeUserId,
                        Subscribeer = new OpenUserDto ()
                        {
                            Id = r.LinUser.Id,
                                Introduction = r.LinUser.Introduction,
                                Nickname = !r.LinUser.IsDeleted ? r.LinUser.Nickname : "该用户已注销",
                                Avatar = r.LinUser.Avatar,
                                Username = r.LinUser.Username,
                        },
                        //当前登录的用户是否关注了这个粉丝
                        IsSubscribeed = _userSubscribeRepository.Select.Any (
                            u => u.CreateUserId == _currentUser.Id && u.SubscribeUserId == r.CreateUserId)
                });

            userSubscribes.ForEach (r => { r.Subscribeer.Avatar = _fileRepository.GetFileUrl (r.Subscribeer.Avatar); });

            return new PagedResultDto<UserSubscribeDto> (userSubscribes, count);
        }

        /// <summary>
        /// 得到某个用户的关注了、关注者、标签总数
        /// </summary>
        /// <param name="userId"></param>
        [HttpGet ("user/{userId}")]
        [AllowAnonymous]
        public SubscribeCountDto GetUserSubscribeInfo (long userId)
        {
            long subscribeCount = _userSubscribeRepository.Select
                .Where (r => r.CreateUserId == userId)
                .Count ();

            long fansCount = _userSubscribeRepository.Select
                .Where (r => r.SubscribeUserId == userId)
                .Count ();

            long tagCount = _userTagRepository.Select.Include (r => r.Tag)
                .Where (r => r.CreateUserId == userId).Count ();

            return new SubscribeCountDto
            {
                SubscribeCount = subscribeCount,
                    FansCount = fansCount,
                    TagCount = tagCount
            };
        }
    }
}