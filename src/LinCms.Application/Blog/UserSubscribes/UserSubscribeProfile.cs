using AutoMapper;
using LinCms.Application.Contracts.Blog.UserSubscribes.Dtos;
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.Blog.UserSubscribes
{

    public class UserSubscribeProfile : Profile
    {
        public UserSubscribeProfile()
        {
            CreateMap<UserSubscribe, UserSubscribeDto>();
        }
    }
}
