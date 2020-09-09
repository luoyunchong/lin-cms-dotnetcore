using System;
using LinCms.Common;
using LinCms.Data;
using LinCms.Data.Enums;
using LinCms.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LinCms.Aop.Filter
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
                UnifyResponseDto warnResponse = new UnifyResponseDto(cmsException.GetErrorCode(), cmsException.Message, context.HttpContext);
                _logger.LogWarning(JsonConvert.SerializeObject(warnResponse));
                HandlerException(context, warnResponse, cmsException.GetCode());
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

            UnifyResponseDto apiResponse = new UnifyResponseDto(ErrorCode.UnknownError, _environment.IsDevelopment() ? error : "服务器正忙，请稍后再试.", context.HttpContext);

            HandlerException(context, apiResponse, StatusCodes.Status500InternalServerError);
        }

        private void HandlerException(ExceptionContext context, UnifyResponseDto apiResponse, int statusCode)
        {
            context.Result = new JsonResult(apiResponse)
            {
                StatusCode = statusCode,
                ContentType = "application/json",
            };
            context.ExceptionHandled = true;
        }
    }
}
