using System;
using System.Collections.Generic;
using System.Text;
using LinCms.Zero.Data.Enums;

namespace LinCms.Zero.Exceptions
{
    [Serializable]
    public class LinCmsException : ApplicationException
    {
        /// <summary>
        /// 状态码
        /// </summary>
        private readonly int _code;
        /// <summary>
        /// 错误码，当为0时，代表正常
        /// </summary>

        private readonly int _errorCode;
        /// <summary>
        /// 
        /// </summary>
        public LinCmsException()
        {
            _errorCode = 0;
            _code = 400;
        }

        public LinCmsException(string message, ErrorCode errorCode, int code = 400):base(message)
        {
            this._errorCode = errorCode.GetHashCode();
            _code = code;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorCode"></param>
        /// <param name="code"></param>
        public LinCmsException(string message, int errorCode = 0, int code = 400)
            : base(message)
        {
            this._errorCode = errorCode;
            _code = code;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        /// <param name="errorCode"></param>
        /// <param name="code"></param>
        public LinCmsException(string message, Exception inner, int errorCode = 0, int code = 400)
            : base(message, inner)
        {
            this._errorCode = errorCode;
            _code = code;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetCode()
        {
            return _code;
        }

        public int GetErrorCode()
        {
            return _errorCode;
        }
    }
}
