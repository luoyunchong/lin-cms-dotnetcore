using System;
using LinCms.Data.Enums;

namespace LinCms.Exceptions
{
    [Serializable]
    public class LinCmsException(string message = "服务器繁忙，请稍后再试!", ErrorCode errorCode = ErrorCode.Fail, int code = 400)
        : ApplicationException(message)
    {
        /// <summary>
        /// 状态码
        /// </summary>
        private readonly int _code = code;
        /// <summary>
        /// 错误码，当为0时，代表正常
        /// </summary>

        private readonly ErrorCode _errorCode = errorCode;
        /// <summary>
        /// 
        /// </summary>
        public LinCmsException() : this("服务器繁忙，请稍后再试!", ErrorCode.Fail, 400)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetCode()
        {
            return _code;
        }

        public ErrorCode GetErrorCode()
        {
            return _errorCode;
        }
    }
}
