using System;
using System.Threading.Tasks;
using FreeSql;
using Microsoft.AspNetCore.Http;

namespace LinCms.Web.Middleware
{
    /// <summary>
    /// 处理FreeSql中同一个请求处于一个事务，并一同提交
    /// </summary>
    public class UnitOfWorkMiddleware
    {
        private readonly RequestDelegate _next;

        public UnitOfWorkMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, IUnitOfWork unitOfWork)
        {
            try
            {
                unitOfWork.Open();
                await _next.Invoke(httpContext);
                unitOfWork.Commit();
            }
            catch
            {
                unitOfWork.Rollback();
                throw;
            }
            finally
            {
                unitOfWork.Dispose();
            }
        }
    }
}
