using Castle.DynamicProxy;

namespace LinCms.Middleware;

public class AopCacheIntercept(AopCacheAsyncIntercept interceptor) : IInterceptor
{
    public void Intercept(IInvocation invocation)
    {
        interceptor.ToInterceptor().Intercept(invocation);
    }
}
