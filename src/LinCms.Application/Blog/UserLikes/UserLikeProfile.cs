using AutoMapper;
using LinCms.Entities.Blog;

namespace LinCms.Blog.UserLikes;

public class UserLikeProfile : Profile
{
    public UserLikeProfile()
    {
        CreateMap<CreateUpdateUserLikeDto, UserLike>();
    }
}