using System;
using FreeSql.DataAnnotations;

namespace LinCms.Core.Entities
{
    /// <summary>
    /// 日志表
    /// </summary>
    [Table(Name = "lin_log")]
    public class LinLog : Entity
    {
        /// <summary>
        /// 访问哪个权限
        /// </summary>
        [Column(DbType = "varchar(100)")]
        public string Authority { get; set; } = string.Empty;

        /// <summary>
        /// 日志信息
        /// </summary>
        [Column(DbType = "varchar(450)")]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// 请求方法
        /// </summary>
        [Column(DbType = "varchar(20)")]
        public string Method { get; set; } = string.Empty;

        /// <summary>
        /// 请求路径
        /// </summary>
        [Column(DbType = "varchar(100)")]
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// 请求的http返回码
        /// </summary>
        public int? StatusCode { get; set; }

        /// <summary>
        /// 日志创建时间
        /// </summary>
        [Column(DbType = "datetime")]
        public DateTime? Time { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 用户当时的昵称
        /// </summary>
        [Column(DbType = "varchar(24)")]
        public string UserName { get; set; } = string.Empty;

        [Column(DbType = "longtext")]
        public string OtherMessage { get; set; }
    }
}
