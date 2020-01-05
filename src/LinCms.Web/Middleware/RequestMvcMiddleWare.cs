using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FreeSql;
using Microsoft.AspNetCore.Http;

namespace LinCms.Web.Middleware
{
    /// <summary>
    /// 处理FreeSql中同一个请求处于一个事务，并一同提交
    /// </summary>
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
            catch (Exception)
            {
                unitOfWork?.Rollback();
                throw;
            }
            finally
            {
                unitOfWork?.Dispose();
            }
        }
    }
}
