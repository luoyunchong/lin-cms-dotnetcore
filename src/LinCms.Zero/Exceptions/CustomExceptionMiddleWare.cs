using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinCms.Zero.Data;
using LinCms.Zero.Data.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LinCms.Zero.Exceptions
{
    public class CustomExceptionMiddleWare
    {
        /// <summary>
        /// 管道请求委托
        /// </summary>
        private readonly RequestDelegate _next;

        private readonly ILogger<CustomExceptionMiddleWare> _logger;

        private IDictionary<int, string> _errCodes;
        private readonly IHostingEnvironment _environment;
        public CustomExceptionMiddleWare(RequestDelegate next, ILogger<CustomExceptionMiddleWare> logger, IHostingEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;

            _errCodes = Enum.GetValues(typeof(ErrorCode))
                .Cast<ErrorCode>()
                .ToDictionary(t => (int)t, t => t.ToString());

        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);//调用管道执行下一个中间件
            }
            catch (Exception ex)
            {
                if (ex is LinCmsException cmsException)//自定义业务异常
                {
                    await JsonHandle(context, cmsException.Message, cmsException.GetErrorCode(), cmsException.GetCode());
                }
                else
                {
                    string errorMsg = _environment.IsDevelopment()
                        ? $"异常信息:{(ex.InnerException != null ? ex.InnerException.Message : ex.Message)}"
                        : "服务器正忙，请稍后再试";

                    await JsonHandle(context, errorMsg, ErrorCode.UnknownError,500);

                }
            }
        }

        /// <summary>
        /// 处理方式：返回Json格式
        /// </summary>
        /// <returns></returns>
        private async Task JsonHandle(HttpContext context, string errorMsg, ErrorCode errorCode,int statusCode)
        {
            _logger.LogError(errorMsg);

            ResultDto apiResponse = new ResultDto()
            {
                Msg = errorMsg,
                ErrorCode = errorCode
            }; ;

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(apiResponse.ToString(), Encoding.UTF8);
        }

    }
}
