using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinCms.Zero.Data;
using LinCms.Zero.Data.Enums;
using LinCms.Zero.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LinCms.Web.Data.Aop
{
    public class LinCmsExceptionFilter : Attribute, IExceptionFilter
    {
        private readonly ILogger logger = null;
        private readonly IHostingEnvironment environment = null;
        public LinCmsExceptionFilter(ILogger<LinCmsExceptionFilter> logger, IHostingEnvironment environment)
        {
            this.logger = logger;
            this.environment = environment;
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

           
            ResultDto apiResponse = new ResultDto()
            {
                ErrorCode = ErrorCode.UnknownError
            };

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

            if (environment.IsDevelopment())
            {
                
                apiResponse.Msg = context.Exception.Message + error;
            }
            else
            {
                apiResponse.Msg = "服务器正忙，请稍后再试";
            }

            HandlerException(context, apiResponse, 500);
        }

        public void HandlerException(ExceptionContext context, ResultDto apiResponse, int statusCode)
        {
            logger.LogError(apiResponse.Msg.ToString());

            ContentResult r = new ContentResult
            {
                StatusCode = statusCode,
                ContentType = "application/json",
                Content = JsonConvert.SerializeObject(apiResponse)
            };


            context.Result = r;
            context.ExceptionHandled = true;
        }
    }
}
