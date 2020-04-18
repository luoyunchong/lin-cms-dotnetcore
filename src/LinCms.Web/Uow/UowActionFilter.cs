using System.Threading.Tasks;
using LinCms.Core.Dependency;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LinCms.Web.Uow
{
    public class UowActionFilter : IAsyncActionFilter, ITransientDependency
    {
        private readonly IFreeSql freeSql;

        public UowActionFilter(IFreeSql freeSql)
        {
            this.freeSql = freeSql;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!(context.ActionDescriptor is ControllerActionDescriptor))
            {
                await next();
                return;
            }

            var methodInfo = (context.ActionDescriptor as ControllerActionDescriptor).MethodInfo;
            var unitOfWorkAttr = UnitOfWorkHelper.GetUnitOfWorkAttributeOrNull(methodInfo);

            if (unitOfWorkAttr?.IsDisabled == true)
            {
                await next();
                return;
            }

            using (var uow = freeSql.CreateUnitOfWork())
            {
                uow.GetOrBeginTransaction();
                var result = await next();
                if (Succeed(result))
                {
                    uow.Commit();
                }
            }
        }

        private static bool Succeed(ActionExecutedContext result)
        {
            return result.Exception == null || result.ExceptionHandled;
        }
    }
}
