using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using FreeSql;
using LinCms.Aop.Attributes;
using Microsoft.Extensions.Logging;

namespace LinCms.Middleware
{
    public class UnitOfWorkInterceptor : IInterceptor
    {
        private readonly UnitOfWorkAsyncInterceptor asyncInterceptor;
        public UnitOfWorkInterceptor(UnitOfWorkAsyncInterceptor interceptor)
        {
            asyncInterceptor = interceptor;
        }

        public void Intercept(IInvocation invocation)
        {
            asyncInterceptor.ToInterceptor().Intercept(invocation);
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
            //_unitOfWork = _unitOfWorkManager.Begin(Propagation.Requierd);
            //return true;
            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            var attribute = method.GetCustomAttributes(typeof(TransactionalAttribute), false).FirstOrDefault();
            if (attribute is TransactionalAttribute transaction)
            {
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
                int? hashCode = _unitOfWork?.GetHashCode();
                try
                {
                    invocation.Proceed();
                    _logger.LogInformation($"----- 拦截同步执行的方法-事务 {hashCode} 提交前----- ");
                    _unitOfWork?.Commit();
                    _logger.LogInformation($"----- 拦截同步执行的方法-事务 {hashCode} 提交成功----- ");
                }
                catch 
                {
                    _logger.LogError($"----- 拦截同步执行的方法-事务 {hashCode} 提交失败----- ");
                    _unitOfWork.Rollback();
                    throw;
                }
      
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
                var methodName = $"{invocation.MethodInvocationTarget.DeclaringType.FullName}.{invocation.Method.Name}()";
                int? hashCode = _unitOfWork?.GetHashCode();

                using (_logger.BeginScope("_unitOfWork:{hashCode}", hashCode))
                {
                    _logger.LogInformation($"----- async Task 开始事务{ hashCode} {methodName}----- ");
                 
                    invocation.Proceed();
                   
                    _ = ((Task)invocation.ReturnValue).ContinueWith(
                    antecedent =>
                    {
                        if (antecedent.Exception == null)
                        {
                            _unitOfWork?.Commit();
                            _logger.LogInformation($"----- async Task 事务 { hashCode} Commit----- ");
                        }
                        else
                        {
                            _unitOfWork?.Rollback();
                            _logger.LogError($"----- async Task 事务 { hashCode} Rollback----- ");
                        }
                    });
                }
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
                var methodName = $"{invocation.MethodInvocationTarget.DeclaringType.FullName}.{invocation.Method.Name}()";
                int? hashCode = _unitOfWork?.GetHashCode();

                _logger.LogInformation($"----- async Task<TResult> 开始事务{ hashCode} {methodName}----- ");

                invocation.Proceed();
             
                var task = (Task<TResult>)invocation.ReturnValue;
                _ = ((Task<TResult>)invocation.ReturnValue).ContinueWith(
                       antecedent =>
                       {
                           if (antecedent.Exception == null)
                           {
                               _unitOfWork?.Commit();
                               _logger.LogInformation($"----- async Task<TResult> Commit事务{ hashCode}----- ");
                           }
                           else
                           {
                               _unitOfWork?.Rollback();
                               _logger.LogError($"----- async Task<TResult> Rollback事务{ hashCode}----- ");
                           }
                       });
            }
            else
            {
                invocation.Proceed();
            }
        }

    }
}

