using System;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using FreeSql;
using LinCms.Core.Aop.Attributes;
using LinCms.Web.Middleware;
using Microsoft.Extensions.Logging;

namespace LinCms.Web.Middleware
{
    public class UnitOfWorkInterceptor : IInterceptor
    {
        private readonly UnitOfWorkAsyncInterceptor _interceptor;
        public UnitOfWorkInterceptor(UnitOfWorkAsyncInterceptor interceptor)
        {
            _interceptor = interceptor;
        }

        public void Intercept(IInvocation invocation)
        {
            _interceptor.ToInterceptor().Intercept(invocation);
        }
    }
    public class UnitOfWorkAsyncInterceptor : IAsyncInterceptor
    {
        private readonly UnitOfWorkManager _unitOfWorkManager;
        private readonly ILogger<UnitOfWorkAsyncInterceptor> _logger;
        IUnitOfWork _unitOfWork;

        public UnitOfWorkAsyncInterceptor(UnitOfWorkManager unitOfWorkManager, ILogger<UnitOfWorkAsyncInterceptor> logger)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _logger = logger;
        }

        private bool TryBegin(IInvocation invocation)
        {
            _unitOfWork = _unitOfWorkManager.Begin(Propagation.Requierd);
            return true;
            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            var attribute = method.GetCustomAttributes(typeof(TransactionalAttribute), false).FirstOrDefault();
            if (attribute is TransactionalAttribute transaction)
            {
                _logger.LogInformation($"事务开启中...");
                _unitOfWork = _unitOfWorkManager.Begin(transaction.Propagation, transaction.IsolationLevel);
                return true;
            }

            return false;
        }
        /// <summary>
        /// 拦截同步执行的方法
        /// </summary>
        /// <param name="invocation"></param>
        public void InterceptSynchronous(IInvocation invocation)
        {
            if (TryBegin(invocation))
            {
                invocation.Proceed();
                OnAfter(null);
            }
            else
            {
                invocation.Proceed();
            }
        }

        /// <summary>
        /// 拦截返回结果为Task的方法
        /// </summary>
        /// <param name="invocation"></param>
        public void InterceptAsynchronous(IInvocation invocation)
        {
            if (TryBegin(invocation))
            {
                invocation.Proceed();
                var returnValue = (Task)invocation.ReturnValue;
                OnAfter(returnValue.Exception);
            }
            else
            {
                invocation.Proceed();
            }
        }

        /// <summary>
        /// 拦截返回结果为Task<TResult>的方法
        /// </summary>
        /// <param name="invocation"></param>
        /// <typeparam name="TResult"></typeparam>
        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            if (TryBegin(invocation))
            {
                invocation.Proceed();
                var task = (Task<TResult>)invocation.ReturnValue;
                OnAfter(task.Exception);
            }
            else
            {
                invocation.Proceed();
            }
        }


        void OnAfter(Exception ex)
        {
            try
            {
                if (ex == null) _unitOfWork.Commit();
                else _unitOfWork.Rollback();
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }

    }
}

