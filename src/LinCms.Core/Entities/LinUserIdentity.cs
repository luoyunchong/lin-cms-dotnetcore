using System;
using FreeSql.DataAnnotations;

namespace LinCms.Core.Entities
{
    [Table(Name = "lin_user_identity")]
    public class LinUserIdentity : Entity<Guid>, ICreateAduitEntity
    {
        public const string GitHub = "GitHub";
        public const string Password = "Password";
        public const string QQ = "QQ";
        public const string WeiXin = "WeiXin";

        /// <summary>
        ///认证类型， GitHub、QQ、WeiXin等
        /// </summary>
        [Column(DbType = "varchar(20)")]
        public string IdentityType { get; set; }
      
        /// <summary>
        /// 认证，例如 用户名,
        /// </summary>
        [Column(DbType = "varchar(24)")]
        public string Identifier { get; set; }

        /// <summary>
        ///  凭证，例如 密码,存OpenId、Id，同一IdentityType的OpenId的值是唯一的
        /// </summary>
        [Column(DbType = "varchar(50)")]
        public string Credential { get; set; }
        /// <summary>
        /// 绑定的用户Id
        /// </summary>
        public long CreateUserId { get; set; }

        [Navigate("CreateUserId")]
        public virtual LinUser LinUser { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
