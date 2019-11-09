using AutoMapper;
using LinCms.Web.Models.v1.UserLikes;
using LinCms.Zero.Domain.Blog;

namespace LinCms.Web.AutoMapper.v1
{
    public class UserLikeProfile : Profile
    {
        public UserLikeProfile()
        {
            CreateMap<CreateUpdateUserLikeDto, UserLike>();
        }
    }
}
