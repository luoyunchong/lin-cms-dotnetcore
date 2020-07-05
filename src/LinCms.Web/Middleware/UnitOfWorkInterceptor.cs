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
            //  return false;
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
                _logger.LogInformation($"事务{0}提交前！！！", invocation.GetHashCode());
                _unitOfWork.Commit();
                _logger.LogInformation($"事务{0}提交成功！！！", invocation.GetHashCode());
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
        public async void InterceptAsynchronous(IInvocation invocation)
        {
            if (TryBegin(invocation))
            {
                invocation.Proceed();
                var task = (Task)invocation.ReturnValue;
                await task;
                OnAfter(task.Exception);
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
        public async void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            if (TryBegin(invocation))
            {
                invocation.Proceed();
                Task task = (Task<TResult>)invocation.ReturnValue;
                await task;
                OnAfter(task.Exception);
            }
            else
            {
                invocation.Proceed();
            }
        }

        void OnAfter(Exception ex)
        {
            if (ex == null)
            {
                _unitOfWork.Commit();
                _logger.LogInformation("OnAfter-Commit", ex);
            }
            else
            {
                _unitOfWork.Rollback();
                _logger.LogInformation("OnAfter-Rollback", ex);
            }
        }
    }
}

