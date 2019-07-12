using System.Net;

namespace LinCms.Web.Data
{
    public class ResultDto
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int ErrorCode { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 请求地址
        /// </summary>
        public string Request { get; set; }
    }
}
