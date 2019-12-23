using System;
using FreeSql.DataAnnotations;

namespace LinCms.Zero.Domain
{
    [Table(Name = "lin_user_community")]
    public class LinUserCommunity : Entity<Guid>, ICreateAduitEntity
    {
        public const string GitHub = "GitHub";
        public const string QQ = "QQ";
        public const string WeiXin = "WeiXin";

        /// <summary>
        /// GitHub、QQ、WeiXin等
        /// </summary>
        [Column(DbType = "varchar(20)")]
        public string IdentityType { get; set; }
        /// <summary>
        /// 存OpenId、Id，同一IdentityType的OpenId的值是唯一的
        /// </summary>
        [Column(DbType = "varchar(50)")]
        public string OpenId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column(DbType = "varchar(24)")]
        public string UserName { get; set; }
        /// <summary>
        ///  个人主页
        /// </summary>
        public string BlogAddress { get; set; }
        /// <summary>
        /// 绑定的用户Id
        /// </summary>
        public long CreateUserId { get; set; }

        [Navigate("CreateUserId")]
        public virtual LinUser LinUser { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
