using LinCms.Data.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LinCms.Cms.Admins
{
    public class UpdateUserDto
    {
        public string Email { get; set; }
        public string Nickname { get; set; }
        public string Username { get; set; }
        public UserActive Active { get; set; }
        public List<long> GroupIds { get; set; }
    }
}
