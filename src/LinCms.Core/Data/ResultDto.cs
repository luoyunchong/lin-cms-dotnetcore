using System;
using LinCms.Core.Common;
using LinCms.Core.Data.Enums;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LinCms.Core.Data
{
    public class ResultDto
    {
    
        /// <summary>
        ///错误码
        /// </summary>
        public ErrorCode ErrorCode { get; set; }

        /// <summary>
        ///错误信息
        /// </summary>
        public object Message { get; set; }

        /// <summary>
        ///请求地址
        /// </summary>
        public string Request { get; set; }

        public ResultDto(ErrorCode errorCode, object message, HttpContext httpContext)
        {
            ErrorCode = errorCode;
            Message = message;
            Request = LinCmsUtils.GetRequest(httpContext);
        }

        public ResultDto(ErrorCode errorCode, object message)
        {
            ErrorCode = errorCode;
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public ResultDto(ErrorCode errorCode)
        {
            ErrorCode = errorCode;
        }

        public ResultDto()
        {
        }

        public static ResultDto Success(string message="操作成功")
        {
            return  new ResultDto(ErrorCode.Success,message);
        }

        public static ResultDto Error(string message="操作失败")
        {
            return new ResultDto(ErrorCode.Fail,message);
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            });
        }
    }
}