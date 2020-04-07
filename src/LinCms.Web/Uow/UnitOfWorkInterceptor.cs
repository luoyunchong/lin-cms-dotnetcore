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
        private readonly IUnitOfWork _unitOfWork;

        public UnitOfWorkInterceptor(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public void Intercept(IInvocation invocation)
        {
            MethodInfo methodInfo = invocation.MethodInvocationTarget;
            if (methodInfo == null)
                methodInfo = invocation.Method;

            UnitOfWorkAttribute transaction = UnitOfWorkHelper.GetUnitOfWorkAttributeOrNull(methodInfo);
            //如果标记了 [UnitOfWork]，并且不在事务嵌套中。
            if (transaction != null && _unitOfWork.Enable)
            {
                //开启事务
                _unitOfWork.Open();
                try
                {
                    invocation.Proceed();
                    _unitOfWork.Commit();
                }
                catch
                {
                    //回滚
                    _unitOfWork.Rollback();
                    throw;
                }
                finally
                {
                    _unitOfWork.Dispose();
                }
            }
            else
            {
                //如果没有标记[UnitOfWork]，直接执行方法
                if (UnitOfWorkHelper.IsAsync(methodInfo))
                {
                    var task = methodInfo.Invoke(invocation.InvocationTarget, invocation.Arguments) as Task;
                    invocation.ReturnValue = task;
                }
                else
                {
                    invocation.Proceed();
                }
            }
        }
    }
}