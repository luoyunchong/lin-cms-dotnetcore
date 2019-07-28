using AutoMapper;
using LinCms.Web.Models.Groups;
using LinCms.Zero.Domain;

namespace LinCms.Web.AutoMapper.Groups
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
