using System.Threading.Tasks;
using LinCms.Core.Dependency;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LinCms.Web.Uow
{
    public class UowActionFilter : IAsyncActionFilter, ITransientDependency
    {
        private readonly IUnitOfWorkManager unitOfWorkManager;

        public UowActionFilter(IUnitOfWorkManager unitOfWorkManager)
        {
            this.unitOfWorkManager = unitOfWorkManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!(context.ActionDescriptor is ControllerActionDescriptor))
            {
                await next();
                return;
            }

            var methodInfo = (context.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo;
            if (methodInfo==null)
            {
                await next();
                return;
            }
            var unitOfWorkAttr = UnitOfWorkHelper.GetUnitOfWorkAttributeOrNull(methodInfo);

            if (unitOfWorkAttr?.IsDisabled == true)
            {
                await next();
                return;
            }

            using var uow = unitOfWorkManager.Begin();
            uow.GetOrBeginTransaction();
            var result = await next();
            if (Succeed(result))
            {
                uow.Commit();
            }
        }

        private static bool Succeed(ActionExecutedContext result)
        {
            return result.Exception == null || result.ExceptionHandled;
        }
    }
}
