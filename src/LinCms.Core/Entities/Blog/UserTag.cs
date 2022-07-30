using System;
using FreeSql.DataAnnotations;

namespace LinCms.Entities.Blog
{
    [Table(Name = "blog_user_tag")]
    public class UserTag : Entity<Guid>, ICreateAuditEntity
    {
        public Guid TagId { get; set; }

        public long CreateUserId { get; set; }

        public DateTime CreateTime { get; set; }

        [Navigate("CreateUserId")]
        public virtual LinUser LinUser { get; set; }

        [Navigate("TagId")]
        public virtual Tag Tag { get; set; }
    }
}
