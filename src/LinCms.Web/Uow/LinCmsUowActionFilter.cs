using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using FreeSql;
using LinCms.Core.Aop;
using LinCms.Core.Dependency;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LinCms.Web.Uow
{
    public class LinCmsUowActionFilter :IAsyncActionFilter, ITransientDependency
    {
        private readonly IUnitOfWork _unitOfWork;
        public LinCmsUowActionFilter(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!(context.ActionDescriptor is ControllerActionDescriptor))
            {
                await next();
                return;
            }

            var methodInfo = (context.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo;
            var unitOfWorkAttr = UnitOfWorkHelper.GetUnitOfWorkAttributeOrNull(methodInfo);
            if (unitOfWorkAttr.IsNotNull()&&unitOfWorkAttr.IsDisabled)
            {
                await next();
                return;
            }

            using (_unitOfWork)
            {
                _unitOfWork.Open();
                var result = await next();
                _unitOfWork.Commit();
                if (!Succeed(result))
                {
                    _unitOfWork.Rollback();
                }
                return;
            }
        }
        private static bool Succeed(ActionExecutedContext result)
        {
            return result.Exception == null || result.ExceptionHandled;
        }
    }


}
