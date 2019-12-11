using System.Linq;
using AutoMapper;
using LinCms.Web.Models.Cms.Users;
using LinCms.Web.Models.v1.UserFollows;
using LinCms.Zero.Data;
using LinCms.Zero.Domain;
using LinCms.Zero.Domain.Blog;
using LinCms.Zero.Exceptions;
using LinCms.Zero.Extensions;
using LinCms.Zero.Repositories;
using LinCms.Zero.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinCms.Web.Controllers.v1
{
    [Route("v1/follow")]
    [ApiController]
    [Authorize]
    public class UserFollowController : ControllerBase
    {
        private readonly AuditBaseRepository<UserFollow> _userFollowRepository;
        private readonly AuditBaseRepository<LinUser> _userRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        public UserFollowController(AuditBaseRepository<UserFollow> userFollowRepository, IMapper mapper, ICurrentUser currentUser, AuditBaseRepository<LinUser> userRepository)
        {
            _userFollowRepository = userFollowRepository;
            _mapper = mapper;
            _currentUser = currentUser;
            _userRepository = userRepository;
        }
        /// <summary>
        /// 判断当前登录的用户是否关注了beFollowUserId
        /// </summary>
        /// <param name="followUserId"></param>
        /// <returns></returns>
        [HttpGet("{followUserId}")]
        public bool Get(long followUserId)
        {
            return _userFollowRepository.Select.Any(r => r.FollowUserId == followUserId && r.CreateUserId == _currentUser.Id);
        }

        /// <summary>
        /// 取消关注
        /// </summary>
        /// <param name="followUserId"></param>
        [HttpDelete("{followUserId}")]
        public void Delete(long followUserId)
        {
            bool any = _userFollowRepository.Select.Any(r => r.CreateUserId == _currentUser.Id && r.FollowUserId == followUserId);
            if (!any)
            {
                throw new LinCmsException("已取消关注");
            }
            _userFollowRepository.Delete(r => r.FollowUserId == followUserId && r.CreateUserId == _currentUser.Id);
        }

        /// <summary>
        /// 关注用户
        /// </summary>
        /// <param name="followUserId"></param>
        [HttpPost("{followUserId}")]
        public void Post(long followUserId)
        {
            LinUser linUser = _userRepository.Select.Where(r => r.Id == followUserId).ToOne();
            if (linUser == null)
            {
                throw new LinCmsException("该用户不存在");
            }

            if (!linUser.IsActive())
            {
                throw new LinCmsException("该用户已被拉黑");
            }

            bool any = _userFollowRepository.Select.Any(r =>
                  r.CreateUserId == _currentUser.Id && r.FollowUserId == followUserId);
            if (any)
            {
                throw new LinCmsException("您已关注该用户");
            }

            UserFollow userFollow = new UserFollow() { FollowUserId = followUserId };
            _userFollowRepository.Insert(userFollow);
        }

        /// <summary>
        /// 得到某个用户的关注
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public PagedResultDto<UserFollowDto> GetUserFolloweeList([FromQuery]UserFollowSearchDto searchDto)
        {
            var userFollows = _userFollowRepository.Select.Include(r => r.FollowUser)
                .Where(r => r.CreateUserId == searchDto.UserId)
                .ToPager(searchDto, out long count)
                .ToList(r => new UserFollowDto
                {
                    CreateUserId = r.CreateUserId,
                    FollowUserId = r.FollowUserId,
                    Follower = new OpenUserDto()
                    {
                        Introduction = r.FollowUser.Introduction,
                        Nickname = r.FollowUser.Nickname,
                        Avatar = r.FollowUser.Avatar,
                        Username = r.FollowUser.Username,
                    },
                    IsFollowed = _userFollowRepository.Select.Any(u =>
                        u.CreateUserId == _currentUser.Id && u.FollowUserId == r.FollowUserId)
                });

            return new PagedResultDto<UserFollowDto>(userFollows, count);
        }


        /// <summary>
        /// 得到某个用户的粉丝
        /// </summary>
        /// <returns></returns>
        [HttpGet("fans")]
        public PagedResultDto<UserFollowDto> GetUserFansList([FromQuery]UserFollowSearchDto searchDto)
        {
            var userFollows = _userFollowRepository.Select.Include(r => r.FollowUser)
                .Where(r => r.FollowUserId == searchDto.UserId)
                .ToPager(searchDto, out long count)
                .ToList(r => new UserFollowDto
                {
                    CreateUserId = r.CreateUserId,
                    FollowUserId = r.FollowUserId,
                    Follower = new OpenUserDto()
                    {
                        Introduction = r.FollowUser.Introduction,
                        Nickname = r.FollowUser.Nickname,
                        Avatar = r.FollowUser.Avatar,
                        Username = r.FollowUser.Username,
                    },
                    IsFollowed = _userFollowRepository.Select.Any(u =>
                        u.CreateUserId == _currentUser.Id && u.FollowUserId == r.FollowUserId)
                });

            return new PagedResultDto<UserFollowDto>(userFollows, count);
        }
    }
}