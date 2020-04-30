using System.Threading.Tasks;
using FreeSql;
using LinCms;
using LinCms.Web;
using LinCms.Web.Middleware;
using Microsoft.AspNetCore.Http;

namespace LinCms.Web.Middleware
{
    /// <summary>
    /// 处理FreeSql中同一个请求处于一个事务，并一同提交
    /// </summary>
    public class UnitOfWorkMiddleware
    {
        private readonly RequestDelegate next;
        public UnitOfWorkMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext, UnitOfWorkManager unitOfWorkManager)
        {
            using IUnitOfWork unitOfWork = unitOfWorkManager.Begin(Propagation.Nested);
            try
            {
                await next.Invoke(httpContext);
                unitOfWork.Commit();
            }
            catch
            {
                unitOfWork.Rollback();
                throw;
            }
            finally
            {
                unitOfWorkManager.Dispose();
            }
        }
    }
}
