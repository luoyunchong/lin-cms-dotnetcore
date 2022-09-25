using System;
using System.ComponentModel.DataAnnotations;
using IGeekFan.FreeKit.Extras.AuditEntity;

namespace LinCms.Entities
{
    /// <summary>
    /// 黑名单，实现登录Token的过期
    /// </summary>
    public class BlackRecord : Entity<Guid>, ICreateAuditEntity<long>
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

        public long? CreateUserId { get; set; }

        public string CreateUserName { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
