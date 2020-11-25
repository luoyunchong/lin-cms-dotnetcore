using Castle.DynamicProxy;
using CSRedis;
using LinCms.Aop.Attributes;
using LinCms.Middleware;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LinCms.Middleware
{

    /// <summary>
    /// Aop
    /// </summary>
    public class AopCacheIntercept : IInterceptor
    {
        public AopCacheIntercept()
        {
        }

        public void Intercept(IInvocation invocation)
        {
            var method = invocation.MethodInvocationTarget ?? invocation.Method;
            //对当前方法的特性验证
            var cacheAttrObj = method.GetCustomAttributes(typeof(AopCacheAttribute), false).FirstOrDefault();
            if (cacheAttrObj is AopCacheAttribute cacheAttr)
            {
                //获取自定义缓存键
                var cacheKey = string.IsNullOrEmpty(cacheAttr.CacheKey) ? GenerateCacheKey(invocation) : cacheAttr.CacheKey;
                //注意是 string 类型，方法GetValue
                var cacheValue = RedisHelper.Get(cacheKey);
                if (cacheValue != null)
                {
                    //将当前获取到的缓存值，赋值给当前执行方法
                    var type = invocation.Method.ReturnType;
                    var resultTypes = type.GenericTypeArguments;
                    if (type.FullName == "System.Void")
                        return;

                    object response;
                    if (type != null && typeof(Task).IsAssignableFrom(type))
                    {
                        //返回Task<T>
                        if (resultTypes.Count() > 0)
                        {
                            var resultType = resultTypes.FirstOrDefault();
                            dynamic temp = JsonConvert.DeserializeObject(cacheValue, resultType);
                            response = Task.FromResult(temp);
                        }
                        else
                            //Task 无返回方法 指定时间内不允许重新运行
                            response = Task.Yield();
                    }
                    else
                        response = Convert.ChangeType(RedisHelper.Get<object>(cacheKey), type);

                    invocation.ReturnValue = response;
                    return;
                }
                //去执行当前的方法
                invocation.Proceed();

                //存入缓存
                if (!string.IsNullOrWhiteSpace(cacheKey))
                {
                    object response;

                    var type = invocation.Method.ReturnType;
                    if (type != null && typeof(Task).IsAssignableFrom(type))
                    {
                        var resultProperty = type.GetProperty("Result");
                        response = resultProperty.GetValue(invocation.ReturnValue);
                    }
                    else
                        response = invocation.ReturnValue;

                    if (response == null) response = string.Empty;
                    RedisHelper.SetAsync(cacheKey, response, exists: cacheAttr.AbsoluteExpires);
                }
            }
            else
                invocation.Proceed();//直接执行被拦截方法
        }

        //自动生成缓存键
        private string GenerateCacheKey(IInvocation invocation)
        {
            var typeName = invocation.TargetType.Name;
            var methodName = invocation.Method.Name;
            var methodArguments = invocation.Arguments.ToList();

            string key = $"{typeName}:{methodName}:";
            string parms = string.Empty;
            int hashCode = methodName.GetHashCode();
            foreach (var param in methodArguments)
            {
                if (param is string || param is int || param is long)
                    parms += param.ToString();
                else if (param is object)
                    parms += JsonConvert.SerializeObject(param).GetHashCode();
                else if (param != null)
                    parms += hashCode ^ param.GetHashCode();
            }
            return string.Concat(key, parms);
        }
    }
}
