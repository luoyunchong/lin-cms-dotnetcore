using AutoMapper;
using LinCms.Entities;

namespace LinCms.Cms.Permissions;

public class PermissionProfile : Profile
{
    public PermissionProfile()
    {
        CreateMap<LinPermission, PermissionDto>();
        CreateMap<LinPermission, TreeNode>();
        CreateMap<LinPermission, PermissionTreeNode>();
        CreateMap<PermissioCreateUpdateDto, LinPermission>();
    }
}