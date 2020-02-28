using System.Collections.Generic;

namespace LinCms.Application.Contracts.Cms.Admins
{
    public class UpdateUserDto
    {
        public string Email { get; set; }
        public string Nickname { get; set; }
        public List<long> GroupIds { get; set; }
    }
}
