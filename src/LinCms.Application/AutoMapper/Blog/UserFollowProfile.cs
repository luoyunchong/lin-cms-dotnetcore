using AutoMapper;
using LinCms.Application.Contracts.Blog.UserSubscribes;
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.AutoMapper.Blog
{

    public class UserSubscribeProfile : Profile
    {
        public UserSubscribeProfile()
        {
            CreateMap<UserSubscribe, UserSubscribeDto>();
        }
    }
}
