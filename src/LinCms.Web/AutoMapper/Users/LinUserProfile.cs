using AutoMapper;
using LinCms.Web.Models.Admins;
using LinCms.Web.Models.Users;
using LinCms.Zero.Domain;

namespace LinCms.Web.AutoMapper.Users
{
    public class LinUserProfile : Profile
    {
        public LinUserProfile()
        {
            CreateMap<UserInputDto,LinUser>();
            CreateMap<LinUser, UserInformation>();
            CreateMap<UpdateUserDto, LinUser>();
        }
    }
}
