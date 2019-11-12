using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinCms.Web.Models.v1.UserLikes
{
    public class CreateUpdateUserLikeDto
    {
        public Guid SubjectId { get; set; }

        public int SubjectType { get; set; }
    }
}
