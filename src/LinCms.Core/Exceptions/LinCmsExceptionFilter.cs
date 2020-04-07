using System;
using LinCms.Core.Common;
using LinCms.Core.Data;
using LinCms.Core.Data.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LinCms.Core.Exceptions
{
    /// <summary>
    /// 出现异常时，如LinCmsException业务异常，会先执行方法过滤器 （LogActionFilterAttribute）的OnActionExecuted才会执行此异常过滤器。
    /// </summary>
    public class LinCmsExceptionFilter : IExceptionFilter
    {
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _environment;
        public LinCmsExceptionFilter(ILogger<LinCmsExceptionFilter> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is LinCmsException cmsException)
            {
                HandlerException(context,
                    new UnifyResponseDto()
                    {
                        Message = cmsException.Message,
                        Code = cmsException.GetErrorCode()
                    },
                    cmsException.GetCode()
                    );
                return;
            }

            string error = "异常信息：";

            void ReadException(Exception ex)
            {
                error += $"{ex.Message} | {ex.StackTrace} | {ex.InnerException}";
                if (ex.InnerException != null)
                {
                    ReadException(ex.InnerException);
                }
            }
            ReadException(context.Exception);

            _logger.LogError(error);

            UnifyResponseDto apiResponse = new UnifyResponseDto()
            {
                Code = ErrorCode.UnknownError,
                Message = _environment.IsDevelopment() ? error : "服务器正忙，请稍后再试."
            };

            HandlerException(context, apiResponse, StatusCodes.Status500InternalServerError);
        }

        private void HandlerException(ExceptionContext context, UnifyResponseDto apiResponse, int statusCode)
        {
            apiResponse.Request = LinCmsUtils.GetRequest(context.HttpContext);

            context.Result = new JsonResult(apiResponse)
            {
                StatusCode = statusCode,
                ContentType = "application/json",
            };
            context.ExceptionHandled = true;
        }
    }
}
