using AutoMapper;
using LinCms.Web.Models.Cms.Admins;
using LinCms.Web.Models.Cms.Users;
using LinCms.Zero.Domain;

namespace LinCms.Web.AutoMapper.Cms
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserDto, LinUser>();
            CreateMap<UpdateUserDto, LinUser>();
            CreateMap<LinUser, UserInformation>();
            CreateMap<LinUser, UserDto>();
        }
    }
}
