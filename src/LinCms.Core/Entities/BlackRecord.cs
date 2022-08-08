using System;
using System.ComponentModel.DataAnnotations;

namespace LinCms.Entities
{
    /// <summary>
    /// 黑名单，实现登录Token的过期
    /// </summary>
    public class BlackRecord : Entity<Guid>, ICreateAuditEntity
    {
        /// <summary>
        /// 用户Token
        /// </summary>
        [StringLength(-2)]
        public string Jti { get; set; }

        /// <summary>
        /// 登录名
        /// </summary>

        [StringLength(50)]
        public string UserName { get; set; }

        public long CreateUserId { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
