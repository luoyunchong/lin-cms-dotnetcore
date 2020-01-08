using AutoMapper;
using LinCms.Application.Contracts.v1.UserLikes;
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.AutoMapper.v1
{
    public class UserLikeProfile : Profile
    {
        public UserLikeProfile()
        {
            CreateMap<CreateUpdateUserLikeDto, UserLike>();
        }
    }
}
