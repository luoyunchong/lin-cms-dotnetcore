using System;
using LinCms.Zero.Common;
using LinCms.Zero.Data;
using LinCms.Zero.Data.Enums;
using LinCms.Zero.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace LinCms.Web.Data.Aop
{
    public class LinCmsExceptionFilter : Attribute, IExceptionFilter
    {
        private readonly ILogger _logger;
        private readonly IHostingEnvironment _environment;
        public LinCmsExceptionFilter(ILogger<LinCmsExceptionFilter> logger, IHostingEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is LinCmsException cmsException)
            {
                HandlerException(context,
                    new ResultDto()
                    {
                        Msg = cmsException.Message,
                        ErrorCode = cmsException.GetErrorCode()
                    },
                    cmsException.GetCode()
                    );
                return;
            }

            string error = string.Empty;

            void ReadException(Exception ex)
            {
                error += $"{ex.Message} | {ex.StackTrace} | {ex.InnerException}";
                if (ex.InnerException != null)
                {
                    ReadException(ex.InnerException);
                }
            }
            ReadException(context.Exception);

            ResultDto apiResponse = new ResultDto()
            {
                ErrorCode = ErrorCode.UnknownError,
                Msg = _environment.IsDevelopment() ?  error : "服务器正忙，请稍后再试"
            };

            HandlerException(context, apiResponse, StatusCodes.Status500InternalServerError);
        }

        private void HandlerException(ExceptionContext context, ResultDto apiResponse, int statusCode)
        {
            apiResponse.Request = LinCmsUtils.GetRequest(context.HttpContext);

            _logger.LogError(apiResponse.ToString());

            context.Result = new JsonResult(apiResponse)
            {
                StatusCode = statusCode,
                ContentType = "application/json",
            };
            context.ExceptionHandled = true;
        }
    }
}
