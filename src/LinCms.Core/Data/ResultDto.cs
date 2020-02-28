using System;
using LinCms.Core.Common;
using LinCms.Core.Data.Enums;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LinCms.Core.Data
{
    public class UnifyResponseDto
    {
    
        /// <summary>
        ///错误码
        /// </summary>
        public ErrorCode Code { get; set; }

        /// <summary>
        ///错误信息
        /// </summary>
        public object Message { get; set; }

        /// <summary>
        ///请求地址
        /// </summary>
        public string Request { get; set; }

        public UnifyResponseDto(ErrorCode errorCode, object message, HttpContext httpContext)
        {
            Code = errorCode;
            Message = message;
            Request = LinCmsUtils.GetRequest(httpContext);
        }

        public UnifyResponseDto(ErrorCode errorCode, object message)
        {
            Code = errorCode;
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public UnifyResponseDto(ErrorCode errorCode)
        {
            Code = errorCode;
        }

        public UnifyResponseDto()
        {
        }

        public static UnifyResponseDto Success(string message="操作成功")
        {
            return  new UnifyResponseDto(ErrorCode.Success,message);
        }

        public static UnifyResponseDto Error(string message="操作失败")
        {
            return new UnifyResponseDto(ErrorCode.Fail,message);
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