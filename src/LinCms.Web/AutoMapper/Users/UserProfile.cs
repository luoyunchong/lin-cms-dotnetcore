using AutoMapper;
using LinCms.Web.Models.Admins;
using LinCms.Web.Models.Users;
using LinCms.Zero.Domain;

namespace LinCms.Web.AutoMapper.Users
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserDto,LinUser>();
            CreateMap<LinUser, UserInformation>();
            CreateMap<UpdateUserDto, LinUser>();
        }
    }
}
