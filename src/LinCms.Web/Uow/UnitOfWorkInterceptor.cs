using System.Reflection;
using Castle.DynamicProxy;
using FreeSql;

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