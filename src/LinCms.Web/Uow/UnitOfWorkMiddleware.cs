using System.Threading.Tasks;
using FreeSql;
using Microsoft.AspNetCore.Http;

namespace LinCms.Web.Uow
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

        public async Task InvokeAsync(HttpContext httpContext, IUnitOfWork unitOfWork)
        {
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
                unitOfWork.Dispose();
            }
        }
    }
}
