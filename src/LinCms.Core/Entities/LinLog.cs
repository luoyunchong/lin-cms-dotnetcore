using System;

using FreeSql.DataAnnotations;

namespace LinCms.Core.Entities
{
    /// <summary>
    /// 日志表
    /// </summary>
    [Table(Name = "lin_log")]
    public class LinLog : FullAduitEntity
    {
        /// <summary>
        /// 访问哪个权限
        /// </summary>
         [Column(StringLength =100)]
        public string Authority { get; set; }

        /// <summary>
        /// 日志信息
        /// </summary>
         [Column(StringLength =500)]
        public string Message { get; set; } 

        /// <summary>
        /// 请求方法
        /// </summary>
         [Column(StringLength =20)]
        public string Method { get; set; } 

        /// <summary>
        /// 请求路径
        /// </summary>
         [Column(StringLength =100)]
        public string Path { get; set; }

        /// <summary>
        /// 请求的http返回码
        /// </summary>
        public int? StatusCode { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 用户当时的昵称
        /// </summary>
         [Column(StringLength =24)]
        public string Username { get; set; }

    }
}
