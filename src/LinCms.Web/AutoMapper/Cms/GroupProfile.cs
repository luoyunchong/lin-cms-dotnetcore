using AutoMapper;
using LinCms.Web.Models.Cms.Groups;
using LinCms.Zero.Domain;

namespace LinCms.Web.AutoMapper.Cms
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
