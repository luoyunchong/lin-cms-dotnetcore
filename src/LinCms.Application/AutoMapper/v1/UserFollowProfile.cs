using AutoMapper;
using LinCms.Application.Contracts.v1.UserSubscribes;
using LinCms.Core.Entities.Blog;

namespace LinCms.Application.AutoMapper.v1
{

    public class UserSubscribeProfile : Profile
    {
        public UserSubscribeProfile()
        {
            CreateMap<UserSubscribe, UserSubscribeDto>();
        }
    }
}
