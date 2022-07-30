using System;
using System.ComponentModel.DataAnnotations;

namespace LinCms.Entities
{
    /// <summary>
    /// 黑名单，实现登录Token的过期
    /// </summary>
    public class BlackRecord : Entity<Guid>, ICreateAuditEntity
    {
        [StringLength(-2)]
        public string Jti { get; set; }

        [StringLength(50)]
        public string UserName { get; set; }

        public long CreateUserId { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
