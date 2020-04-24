using AutoMapper;
using LinCms.Application.Contracts.Cms.Permissions.Dtos;
using LinCms.Core.Entities;

namespace LinCms.Application.Cms.Permissions
{
    public class PermissionProfile : Profile
    {
        public PermissionProfile()
        {
            CreateMap<LinPermission,PermissionDto>();
        }
    }
}
