using Castle.DynamicProxy;

namespace LinCms.Middleware
{
    public class UnitOfWorkInterceptor : IInterceptor
    {
        private readonly UnitOfWorkAsyncInterceptor _asyncInterceptor;

        public UnitOfWorkInterceptor(UnitOfWorkAsyncInterceptor interceptor)
        {
            _asyncInterceptor = interceptor;
        }

        public void Intercept(IInvocation invocation)
        {
            _asyncInterceptor.ToInterceptor().Intercept(invocation);
        }
    }
}