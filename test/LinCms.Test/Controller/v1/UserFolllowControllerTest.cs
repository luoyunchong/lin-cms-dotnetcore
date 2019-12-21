using System.Linq;
using AutoMapper;
using LinCms.Web.Models.Cms.Users;
using LinCms.Web.Models.v1.UserSubscribes;
using LinCms.Zero.Data;
using LinCms.Zero.Domain.Blog;
using LinCms.Zero.Extensions;
using LinCms.Zero.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LinCms.Test.Controller.v1
{
    public class UserFolllowControllerTest : BaseControllerTests
    {
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly IMapper _mapper;
        private readonly IFreeSql _freeSql;
        private readonly AuditBaseRepository<UserSubscribe> _userSubscribeRepository;
        public UserFolllowControllerTest() : base()
        {
            _hostingEnv = serviceProvider.GetService<IWebHostEnvironment>();

            _mapper = serviceProvider.GetService<IMapper>();
            _userSubscribeRepository = serviceProvider.GetService<AuditBaseRepository<UserSubscribe>>();
            _freeSql = serviceProvider.GetService<IFreeSql>();
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
