using AutoMapper;
using LinCms.Application.Contracts.Blog.UserLikes;
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.AutoMapper.Blog
{
    public class UserLikeProfile : Profile
    {
        public UserLikeProfile()
        {
            CreateMap<CreateUpdateUserLikeDto, UserLike>();
        }
    }
}
