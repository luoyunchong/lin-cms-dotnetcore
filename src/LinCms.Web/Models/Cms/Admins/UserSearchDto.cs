using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Zero.Data;

namespace LinCms.Web.Models.Admins
{
    public class UserSearchDto:PageDto
    {
        public int? GroupId { get; set; }
    }
}
