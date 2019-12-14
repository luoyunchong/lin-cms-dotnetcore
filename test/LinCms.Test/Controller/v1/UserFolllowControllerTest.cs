using System.Linq;
using AutoMapper;
using LinCms.Web.Models.Cms.Users;
using LinCms.Web.Models.v1.UserFollows;
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
        private readonly AuditBaseRepository<UserFollow> _userFollowRepository;
        public UserFolllowControllerTest() : base()
        {
            _hostingEnv = serviceProvider.GetService<IWebHostEnvironment>();

            _mapper = serviceProvider.GetService<IMapper>();
            _userFollowRepository = serviceProvider.GetService<AuditBaseRepository<UserFollow>>();
            _freeSql = serviceProvider.GetService<IFreeSql>();
        }

        [Fact]
        public void GetUserFolloweeList()
        {
            UserFollowSearchDto searchDto = new UserFollowSearchDto()
            {
                UserId = 11
            };
            //.Include(r => r.BeFollowUser)
            var userFollows = _userFollowRepository.Select
                .Where(r => r.CreateUserId == searchDto.UserId)
                .ToList(r => new
                {
                    CreateUserId = r.CreateUserId,
                    BeFollowUserId = r.FollowUserId,
                    Follower = r.FollowUser,
                    IsFollowed = _userFollowRepository.Select.Any(u =>
                        u.CreateUserId == 7 && u.FollowUserId == r.FollowUserId)
                });

            var userFollow2 = _userFollowRepository.Select
                .Where(r => r.CreateUserId == searchDto.UserId)
                .ToList(r => new UserFollowDto()
                {
                    CreateUserId = r.CreateUserId,
                    FollowUserId = r.FollowUserId,
                    Follower = new OpenUserDto()
                    {
                        Id = r.FollowUser.Id,
                        Introduction = r.FollowUser.Introduction,
                        Nickname = r.FollowUser.Nickname,
                        Avatar = r.FollowUser.Avatar,
                        Username = r.FollowUser.Username,
                    },
                    IsFollowed = _userFollowRepository.Select.Any(u =>
                        u.CreateUserId == 7 && u.FollowUserId == r.FollowUserId)
                });



        }
    }
}
