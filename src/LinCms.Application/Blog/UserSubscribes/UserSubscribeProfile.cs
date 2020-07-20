using AutoMapper;
using LinCms.Entities.Blog;

namespace LinCms.Blog.UserSubscribes
{

    public class UserSubscribeProfile : Profile
    {
        public UserSubscribeProfile()
        {
            CreateMap<UserSubscribe, UserSubscribeDto>();
        }
    }
}
