using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FreeSql;
using Microsoft.AspNetCore.Http;

namespace LinCms.Web.Middleware
{
    public class RequestMvcMiddleWare
    {
        private readonly RequestDelegate _next;

        public RequestMvcMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context,IUnitOfWork unitOfWork)
        {
            try
            {
                unitOfWork.Open();
                await _next(context);
                unitOfWork.Commit();

            }
            catch (Exception ex)
            {
                unitOfWork?.Rollback();
                throw ex;
            }
            finally
            {
                unitOfWork?.Dispose();
            }
            // Call the next delegate/middleware in the pipeline
        }
    }
}
