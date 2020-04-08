using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using FreeSql;
using LinCms.Core.Aop;

namespace LinCms.Web.Uow
{
    public class UnitOfWorkInterceptor : IInterceptor
    {
        private readonly IUnitOfWorkManager unitOfWorkManager;

        public UnitOfWorkInterceptor(IUnitOfWorkManager unitOfWorkManager)
        {
            this.unitOfWorkManager = unitOfWorkManager;
        }

        public void Intercept(IInvocation invocation)
        {
            MethodInfo methodInfo = invocation.MethodInvocationTarget;
            if (methodInfo == null)
                methodInfo = invocation.Method;

            //如果标记了 [UnitOfWork]
            if (!UnitOfWorkHelper.IsUnitOfWorkMethod(methodInfo, out var unitOfWorkAttribute))
            {
                using (IUnitOfWork unitOfWork = unitOfWorkManager.Begin())
                {
                    try
                    {
                        invocation.Proceed();
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
            else
            {
                invocation.Proceed();
            }
        }
    }
}