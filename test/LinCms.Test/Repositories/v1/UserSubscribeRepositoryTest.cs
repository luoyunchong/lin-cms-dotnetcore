﻿using AutoMapper;
using LinCms.Web.Models.Cms.Users;
using LinCms.Web.Models.v1.UserSubscribes;
using LinCms.Zero.Domain.Blog;
using LinCms.Zero.Repositories;
using Microsoft.AspNetCore.Hosting;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace LinCms.Test.Repositories.v1
{
    public class UserSubscribeRepositoryTest : BaseRepositoryTest
    {
        private readonly AuditBaseRepository<UserSubscribe> _userSubscribeRepository;
        public UserSubscribeRepositoryTest() : base()
        {
            _userSubscribeRepository = ServiceProvider.GetService<AuditBaseRepository<UserSubscribe>>();
        }

        [Fact]
        public void GetUserSubscribeeeList()
        {
            UserSubscribeSearchDto searchDto = new UserSubscribeSearchDto()
            {
                UserId = 11
            };
            //.Include(r => r.BeSubscribeUser)
            var userSubscribes = _userSubscribeRepository.Select
                .Where(r => r.CreateUserId == searchDto.UserId)
                .ToList(r => new
                {
                    CreateUserId = r.CreateUserId,
                    BeSubscribeUserId = r.SubscribeUserId,
                    Subscribeer = r.SubscribeUser,
                    IsSubscribeed = _userSubscribeRepository.Select.Any(u =>
                        u.CreateUserId == 7 && u.SubscribeUserId == r.SubscribeUserId)
                });

            var userSubscribe2 = _userSubscribeRepository.Select
                .Where(r => r.CreateUserId == searchDto.UserId)
                .ToList(r => new UserSubscribeDto()
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
                        u.CreateUserId == 7 && u.SubscribeUserId == r.SubscribeUserId)
                });
        }
    }
}
