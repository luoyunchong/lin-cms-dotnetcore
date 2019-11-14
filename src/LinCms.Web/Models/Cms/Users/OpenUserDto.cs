using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Zero.Domain;

namespace LinCms.Web.Models.Cms.Users
{
    public class OpenUserDto : EntityDto
    {
        public string Username { get; set; }
        public string Nickname { get; set; }
        public string Avatar { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
