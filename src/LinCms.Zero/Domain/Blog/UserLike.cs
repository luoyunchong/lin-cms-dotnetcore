using System;
using System.Collections.Generic;
using System.Text;

namespace LinCms.Zero.Domain.Blog
{
    public class UserLike : FullAduitEntity<Guid>
    {
        public Guid ArticleId { get; set; }
    }
}
