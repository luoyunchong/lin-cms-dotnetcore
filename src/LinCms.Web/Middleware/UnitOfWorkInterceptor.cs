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
}