using LinCms.Application.Contracts.Blog.UserSubscribes;
using LinCms.Application.Contracts.Blog.UserSubscribes.Dtos;
using LinCms.Core.Entities.Blog;
using LinCms.Application.Contracts.Cms.Users;
using LinCms.Application.Contracts.Cms.Users.Dtos;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using LinCms.Core.IRepositories;

namespace LinCms.Test.Repositories.Blog
{
    public class UserSubscribeRepositoryTest : BaseLinCmsTest
    {
        private readonly IAuditBaseRepository<UserSubscribe> _userSubscribeRepository;
        public UserSubscribeRepositoryTest() : base()
        {
            _userSubscribeRepository = ServiceProvider.GetService<IAuditBaseRepository<UserSubscribe>>();
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
