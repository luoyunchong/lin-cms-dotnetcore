using LinCms.Core.Data;

namespace LinCms.Application.Contracts.Cms.Admins.Dtos
{
    public class UserSearchDto:PageDto
    {
        public int? GroupId { get; set; }
    }
}
