using AutoMapper;
using LinCms.Application.Contracts.Cms.Account;
using LinCms.Application.Contracts.Cms.Admins.Dtos;
using LinCms.Application.Contracts.Cms.Users.Dtos;
using LinCms.Core.Entities;

namespace LinCms.Application.Cms.Users
{
    public class UserIdentityProfile : Profile
    {
        public UserIdentityProfile()
        {
            CreateMap<LinUserIdentity, UserIdentityDto>();

        }
    }
}
