using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using JetBrains.Annotations;
using LinCms.Core.Aop;

namespace LinCms.Web.Uow
{
    public  class UnitOfWorkHelper
    {
        public static object CallGenericMethod(IInvocation invocation, Action<object> callBackAction, Action<Exception> exceptionAction)
        {
            return typeof(UnitOfWorkHelper)
                .GetMethod("ExecuteGenericMethod", BindingFlags.Public | BindingFlags.Static)
                ?.MakeGenericMethod(invocation.Method.ReturnType.GenericTypeArguments[0])
                .Invoke(null, new object[] { invocation.ReturnValue, callBackAction, exceptionAction });
        }
        
        public static bool IsAsync(MethodInfo method)
        {
            return method.ReturnType == typeof(Task)
                   || (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>));
        }
        
    }

}
