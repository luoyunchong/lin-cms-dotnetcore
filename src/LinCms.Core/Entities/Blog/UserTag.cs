using System;
using FreeSql.DataAnnotations;
using IGeekFan.FreeKit.Extras.AuditEntity;

namespace LinCms.Entities.Blog
{
    /// <summary>
    /// 用户关注的标签
    /// </summary>
    [Table(Name = "blog_user_tag")]
    public class UserTag : Entity<Guid>, ICreateAuditEntity<long>
    {
        public Guid TagId { get; set; }

        public long? CreateUserId { get; set; }
        public string CreateUserName { get; set; }


        public DateTime CreateTime { get; set; }

        [Navigate("CreateUserId")]
        public virtual LinUser LinUser { get; set; }

        [Navigate("TagId")]
        public virtual Tag Tag { get; set; }
    }
}
