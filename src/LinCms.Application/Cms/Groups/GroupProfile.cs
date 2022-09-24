using AutoMapper;
using LinCms.Entities;

namespace LinCms.Cms.Groups;

public class GroupProfile : Profile
{
    public GroupProfile()
    {
        CreateMap<CreateGroupDto, LinGroup>();
        CreateMap<UpdateGroupDto, LinGroup>();
        CreateMap<LinGroup, GroupDto>();
    }
}