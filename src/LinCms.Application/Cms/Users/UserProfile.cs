using AutoMapper;
using LinCms.Cms.Account;
using LinCms.Cms.Admins;
using LinCms.Entities;

namespace LinCms.Cms.Users;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserDto, LinUser>();
        CreateMap<UpdateUserDto, LinUser>();
        CreateMap<LinUser, UserInformation>();
        CreateMap<LinUser, UserDto>();
        CreateMap<LinUser, OpenUserDto>();
        CreateMap<LinUser, UserNoviceDto>();
        CreateMap<RegisterDto, LinUser>();
    }
}