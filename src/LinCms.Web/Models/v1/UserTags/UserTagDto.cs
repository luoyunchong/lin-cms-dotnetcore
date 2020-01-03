using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinCms.Web.Models.Cms.Users;

namespace LinCms.Web.Models.v1.UserTags
{
    public class UserTagDto
    {
        public Guid TagId { get; set; }
        public long CreateUserId { get; set; }
        public bool IsSubscribeed { get; set; }
    }
}
