using System;

namespace LinCms.Zero.Data
{
    public class ResultDto
    {
        /// <summary>
        ///     错误码
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        ///     错误信息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        ///     请求地址
        /// </summary>
        public string Request { get; set; }

        public ResultDto(int errorCode, string msg)
        {
            ErrorCode = errorCode;
            Msg = msg ?? throw new ArgumentNullException(nameof(msg));
        }

        public ResultDto(int errorCode)
        {
            ErrorCode = errorCode;
        }

        public ResultDto()
        {
        }

        public ResultDto(string msg)
        {
            Msg = msg ?? throw new ArgumentNullException(nameof(msg));
        }

    }
}