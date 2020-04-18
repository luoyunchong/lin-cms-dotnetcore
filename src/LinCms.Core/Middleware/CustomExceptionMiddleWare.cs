using System;
using System.Text;
using System.Threading.Tasks;
using LinCms.Core.Common;
using LinCms.Core.Data;
using LinCms.Core.Data.Enums;
using LinCms.Core.Dependency;
using LinCms.Core.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LinCms.Core.Middleware
{
    /// <summary>
    /// 如果使用中间件处理异常，当异常处理后，之后 会再去执行LogActionFilterAttriute中的OnActionExecuted方法
    /// </summary>
    public class CustomExceptionMiddleWare : IMiddleware
    {
        private readonly ILogger<CustomExceptionMiddleWare> _logger;
        private readonly IWebHostEnvironment _environment;
        public CustomExceptionMiddleWare(ILogger<CustomExceptionMiddleWare> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context); //调用管道执行下一个中间件
            }
            catch (Exception ex)
            {
                if (ex is LinCmsException cmsException) //自定义业务异常
                {
                    await JsonHandle(context, cmsException.Message, cmsException.GetErrorCode(),
                        cmsException.GetCode());
                }
                else
                {

                    _logger.LogError(ex, "系统异常信息");
                    if (_environment.IsDevelopment())
                    {
                        string errorMsg = "异常信息：";

                        void ReadException(Exception ex)
                        {
                            errorMsg += $"{ex.Message} | {ex.StackTrace}";
                            if (ex.InnerException != null)
                            {
                                ReadException(ex.InnerException);
                            }
                        }
                        ReadException(ex);
                        await JsonHandle(context, errorMsg, ErrorCode.UnknownError, 500);
                    }
                    else
                    {
                        await JsonHandle(context, "服务器正忙，请稍后再试!", ErrorCode.UnknownError, 500);
                    }
                }
            }
        }


        /// <summary>
        /// 处理方式：返回Json格式
        /// </summary>
        /// <returns></returns>
        private async Task JsonHandle(HttpContext context, string errorMsg, ErrorCode errorCode, int statusCode)
        {
            UnifyResponseDto apiResponse = new UnifyResponseDto()
            {
                Message = errorMsg,
                Code = errorCode,
                Request = LinCmsUtils.GetRequest(context)
            }; 

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(apiResponse.ToString(), Encoding.UTF8); ;
        }

    }
}
