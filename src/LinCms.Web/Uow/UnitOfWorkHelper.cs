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
        
        
        public static bool IsUnitOfWorkType(TypeInfo implementationType)
        {
            //Explicitly defined UnitOfWorkAttribute
            if (HasUnitOfWorkAttribute(implementationType) || AnyMethodHasUnitOfWorkAttribute(implementationType))
            {
                return true;
            }

            return false;
        }

        public static UnitOfWorkAttribute GetUnitOfWorkAttributeOrNull(MethodInfo methodInfo)
        {
            var attrs = methodInfo.GetCustomAttributes(true).OfType<UnitOfWorkAttribute>().ToArray();
            if (attrs.Length > 0)
            {
                return attrs[0];
            }

            attrs = methodInfo.DeclaringType?.GetTypeInfo()?.GetCustomAttributes(true)?.OfType<UnitOfWorkAttribute>()?.ToArray();
            if (attrs.Length > 0)
            {
                return attrs[0];
            }

            return null;
        }

        public static bool IsUnitOfWorkMethod([NotNull] MethodInfo methodInfo, [CanBeNull] out UnitOfWorkAttribute unitOfWorkAttribute)
        {

            //Method declaration
            var attrs = methodInfo.GetCustomAttributes(true).OfType<UnitOfWorkAttribute>().ToArray();
            if (attrs.Any())
            {
                unitOfWorkAttribute = attrs.First();
                return !unitOfWorkAttribute.IsDisabled;
            }

            if (methodInfo.DeclaringType != null)
            {
                //Class declaration
                attrs = methodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes(true).OfType<UnitOfWorkAttribute>().ToArray();
                if (attrs.Any())
                {
                    unitOfWorkAttribute = attrs.First();
                    return !unitOfWorkAttribute.IsDisabled;
                }
            }

            unitOfWorkAttribute = null;
            return false;
        }
        private static bool AnyMethodHasUnitOfWorkAttribute(TypeInfo implementationType)
        {
            return implementationType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Any(HasUnitOfWorkAttribute);
        }

        private static bool HasUnitOfWorkAttribute(MemberInfo methodInfo)
        {
            return methodInfo.IsDefined(typeof(UnitOfWorkAttribute), true);
        }
    }

}
