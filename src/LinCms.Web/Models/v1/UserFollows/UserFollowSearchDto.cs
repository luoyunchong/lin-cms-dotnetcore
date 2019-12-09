using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Zero.Data;

namespace LinCms.Web.Models.v1.UserFollows
{
    public class UserFollowSearchDto:PageDto
    {
        public long UserId { get; set; }
    }
}
