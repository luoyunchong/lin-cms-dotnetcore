using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;

namespace LinCms.Entities
{
    /// <summary>
    /// 用户身份认证登录表
    /// </summary>
    [Table(Name = "lin_user_identity")]
    public class LinUserIdentity : FullAduitEntity<Guid>
    {
        public const string GitHub = "GitHub";
        public const string Password = "Password";
        public const string QQ = "QQ";
        public const string Gitee = "Gitee";
        public const string WeiXin = "WeiXin";

        public LinUserIdentity()
        {
        }

        public LinUserIdentity(string identityType, string identifier, string credential, DateTime createTime)
        {
            IdentityType = identityType ?? throw new ArgumentNullException(nameof(identityType));
            Identifier = identifier;
            Credential = credential ?? throw new ArgumentNullException(nameof(credential));
            CreateTime = createTime;
        }

        /// <summary>
        ///认证类型， Password，GitHub、QQ、WeiXin等
        /// </summary>
        [Column(StringLength = 20)]
        public string IdentityType { get; set; }

        /// <summary>
        /// 认证者，例如 用户名,手机号，邮件等，
        /// </summary>
        [Column(StringLength = 24)]
        public string Identifier { get; set; }

        /// <summary>
        ///  凭证，例如 密码,存OpenId、Id，同一IdentityType的OpenId的值是唯一的
        /// </summary>
        [Column(StringLength = 50)]
        public string Credential { get; set; }


        public string ExtraProperties { get; set; }

    }

    public class ExtraProperties : Dictionary<string, string>
    {

    }
}
