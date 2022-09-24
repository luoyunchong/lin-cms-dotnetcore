using AutoMapper;
using LinCms.Entities;

namespace LinCms.Cms.Users;

public class UserIdentityProfile : Profile
{
    public UserIdentityProfile()
    {
        CreateMap<LinUserIdentity, UserIdentityDto>();

    }
}