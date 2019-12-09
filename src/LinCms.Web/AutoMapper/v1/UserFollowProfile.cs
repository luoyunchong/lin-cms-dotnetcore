using AutoMapper;
using LinCms.Web.Models.v1.UserFollows;
using LinCms.Zero.Domain.Blog;

namespace LinCms.Web.AutoMapper.v1
{

    public class UserFollowProfile : Profile
    {
        public UserFollowProfile()
        {
            CreateMap<UserFollow, UserFollowDto>();
        }
    }
}
