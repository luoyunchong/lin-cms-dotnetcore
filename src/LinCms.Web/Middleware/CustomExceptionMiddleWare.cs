using System;
using System.Text;
using System.Threading.Tasks;
using LinCms.Zero.Common;
using LinCms.Zero.Data;
using LinCms.Zero.Data.Enums;
using LinCms.Zero.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LinCms.Web.Middleware
{
    /// <summary>
    /// 如果使用中间件处理异常，当异常处理后，之后 会再去执行LogActionFilterAttriute中的OnActionExecuted方法
    /// </summary>
    public class CustomExceptionMiddleWare
    {
        /// <summary>
        /// 管道请求委托
        /// </summary>
        private readonly RequestDelegate _next;

        private readonly ILogger<CustomExceptionMiddleWare> _logger;

        private readonly IWebHostEnvironment _environment;
        public CustomExceptionMiddleWare(RequestDelegate next, ILogger<CustomExceptionMiddleWare> logger, IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
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
                ErrorCode = errorCode ,
                Request = LinCmsUtils.GetRequest(context)
            }; ;

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(apiResponse.ToString(), Encoding.UTF8); ;
        }

    }
}
