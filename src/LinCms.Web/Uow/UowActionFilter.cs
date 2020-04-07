using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FreeSql;
using LinCms.Core.Dependency;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LinCms.Web.Uow
{
    public class UowActionFilter : IAsyncActionFilter, ITransientDependency
    {
        private readonly IUnitOfWork unitOfWork;

        public UowActionFilter(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
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
            using (var uow = unitOfWork.GetOrBeginTransaction())
            {
                unitOfWork.Open();
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
