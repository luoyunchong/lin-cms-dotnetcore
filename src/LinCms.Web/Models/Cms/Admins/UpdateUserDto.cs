using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinCms.Web.Models.Admins
{
    public class UpdateUserDto
    {
        public string Email { get; set; }
        public int GroupId { get; set; }
    }
}
