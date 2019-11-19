using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;

namespace LinCms.Zero.Domain.Blog
{
    [Table(Name = "blog_user_tag")]
    public class UserTag : Entity<Guid>
    {
        public Guid TagId { get; set; }
        public Guid UserId { get; set; }
        public virtual ICollection<Tag> Tags { get; set; }
        public virtual ICollection<LinUser> LinUser { get; set; }
    }
}
