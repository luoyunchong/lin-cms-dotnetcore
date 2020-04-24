using AutoMapper;
using LinCms.Application.Contracts.Cms.Groups.Dtos;
using LinCms.Core.Entities;

namespace LinCms.Application.Cms.Groups
{
    public class GroupProfile:Profile
    {
        public GroupProfile()
        {
            CreateMap<CreateGroupDto, LinGroup>();
            CreateMap<UpdateGroupDto, LinGroup>();
            CreateMap<LinGroup, GroupDto>();
        }
    }
}
