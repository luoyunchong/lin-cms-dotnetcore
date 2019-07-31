using System;
using LinCms.Zero.Common;
using LinCms.Zero.Data.Enums;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LinCms.Zero.Data
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
        public object Msg { get; set; }

        /// <summary>
        ///请求地址
        /// </summary>
        public string Request { get; set; }

        public ResultDto(ErrorCode errorCode, object msg, HttpContext httpContext)
        {
            ErrorCode = errorCode;
            Msg = msg;
            Request = LinCmsUtils.GetRequest(httpContext);
        }

        public ResultDto(ErrorCode errorCode, object msg)
        {
            ErrorCode = errorCode;
            Msg = msg ?? throw new ArgumentNullException(nameof(msg));
        }

        public ResultDto(ErrorCode errorCode)
        {
            ErrorCode = errorCode;
        }

        public ResultDto()
        {
        }

        public static ResultDto Success(string msg="操作成功")
        {
            return  new ResultDto(ErrorCode.Success,msg);
        }

        public static ResultDto Error(string msg="操作失败")
        {
            return new ResultDto(ErrorCode.Fail,msg);
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