using Castle.DynamicProxy;

namespace LinCms.Middleware;

public class AopCacheIntercept : IInterceptor
{
    private readonly AopCacheAsyncIntercept _asyncInterceptor;

    public AopCacheIntercept(AopCacheAsyncIntercept interceptor)
    {
        _asyncInterceptor = interceptor;
    }

    public void Intercept(IInvocation invocation)
    {
        _asyncInterceptor.ToInterceptor().Intercept(invocation);
    }
}
