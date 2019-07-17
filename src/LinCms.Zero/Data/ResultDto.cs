using System;
using LinCms.Zero.Data.Enums;

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
        public object Msg { get; set; }

        /// <summary>
        ///     请求地址
        /// </summary>
        public string Request { get; set; }

        public ResultDto(int errorCode, object msg)
        {
            ErrorCode = errorCode;
            Msg = msg ?? throw new ArgumentNullException(nameof(msg));
        }

        public ResultDto(ErrorCode errorCode, object msg)
        {
            ErrorCode = errorCode.GetHashCode();
            Msg = msg ?? throw new ArgumentNullException(nameof(msg));
        }

        public ResultDto(int errorCode)
        {
            ErrorCode = errorCode;
        }

        public ResultDto(ErrorCode errorCode)
        {
            ErrorCode = errorCode.GetHashCode();
        }

        public ResultDto()
        {
        }


        public static ResultDto Success(string msg="操作成功")
        {
            return  new ResultDto(0,msg);
        }

        public static ResultDto Error(string msg="操作失败")
        {
            return new ResultDto(400,msg);
        }
    }
}