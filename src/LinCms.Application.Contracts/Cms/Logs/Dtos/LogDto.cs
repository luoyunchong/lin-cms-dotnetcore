using System;
using System.Collections.Generic;
using System.Text;
using LinCms.Core.Entities;

namespace LinCms.Application.Contracts.Cms.Logs.Dtos
{
    public class LogDto
    {
        /// <summary>
        /// 访问哪个权限
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// 日志信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 请求方法
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// 请求路径
        /// </summary>
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
        public string Username { get; set; }
    }
}
