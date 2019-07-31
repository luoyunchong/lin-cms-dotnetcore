using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Zero.Domain;

namespace LinCms.Web.Models.Users
{
    public class UserDto:EntityDto
    {
        public string Nickname { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public int Admin { get; set; } = 1;
        public int Active { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
