using AutoMapper;
using LinCms.Web.Models.v1.UserSubscribes;
using LinCms.Zero.Domain.Blog;

namespace LinCms.Web.AutoMapper.v1
{

    public class UserSubscribeProfile : Profile
    {
        public UserSubscribeProfile()
        {
            CreateMap<UserSubscribe, UserSubscribeDto>();
        }
    }
}
