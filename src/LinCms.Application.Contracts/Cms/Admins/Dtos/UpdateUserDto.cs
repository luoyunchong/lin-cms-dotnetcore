using System.Collections.Generic;

namespace LinCms.Application.Contracts.Cms.Admins.Dtos
{
    public class UpdateUserDto
    {
        public string Email { get; set; }
        public string Nickname { get; set; }
        public List<long> GroupIds { get; set; }
    }
}
