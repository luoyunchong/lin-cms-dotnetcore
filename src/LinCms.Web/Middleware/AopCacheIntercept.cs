using Castle.DynamicProxy;
using LinCms.Aop.Attributes;
using LinCms.Common;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LinCms.Middleware
{

    /// <summary>
    /// Aop
    /// </summary>
    public class AopCacheIntercept : IInterceptor
    {
        private readonly IConfiguration _configuration;

        public AopCacheIntercept(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public void Intercept(IInvocation invocation)
        {
            try
            {
                bool isEnable = bool.Parse(_configuration["Cache:Enable"]);
                Type returnType = invocation.Method.ReturnType;

                if (isEnable == false || returnType == typeof(void))
                {
                    invocation.Proceed();
                    return;
                }

                MethodInfo method = invocation.MethodInvocationTarget ?? invocation.Method;

                //对当前方法的特性验证
                var cacheAttrObj = method.GetCustomAttributes(typeof(CacheableAttribute), false).FirstOrDefault();

                if (cacheAttrObj is CacheableAttribute cacheAttr)
                {
                    //获取自定义缓存键
                    string cacheKey = GenerateCacheKey(cacheAttr.CacheKey, invocation);

                    string cacheValue = RedisHelper.Get(cacheKey);
                    if (cacheValue != null)
                    {
                        //将当前获取到的缓存值，赋值给当前执行方法
                        Type[] resultTypes = returnType.GenericTypeArguments;

                        if (returnType == typeof(Task))
                        {
                            invocation.ReturnValue = InterceptAsync(cacheKey, (Task)invocation.ReturnValue);
                        }
                        else if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
                        {
                            dynamic d = JsonConvert.DeserializeObject(cacheValue, resultTypes.FirstOrDefault());
                            invocation.ReturnValue = Task.FromResult(d);
                        }
                        else
                        {
                            invocation.ReturnValue = JsonConvert.DeserializeObject(cacheValue, returnType);
                        }

                        return;
                    }

                    invocation.Proceed();

                    if (returnType == typeof(Task))
                    {
                        invocation.ReturnValue = InterceptAsync(cacheKey, (Task)invocation.ReturnValue);
                    }
                    else if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
                    {
                        invocation.ReturnValue = InterceptAsync(cacheKey, (dynamic)invocation.ReturnValue);
                    }
                    else
                    {
                        int ExpireSeconds = int.Parse(_configuration["Cache:ExpireSeconds"].ToString());

                        RedisHelper.Set(cacheKey, JsonConvert.SerializeObject(invocation.ReturnValue), ExpireSeconds);
                    }
                    return;
                }

                invocation.Proceed();
            }
            catch (Exception)
            {
                throw;
            }
        }

        // do the continuation work for Task...
        private async Task InterceptAsync(string cacheKey, Task task)
        {
            await task.ConfigureAwait(false);
        }

        // do the continuation work for Task<T>...
        private async Task<T> InterceptAsync<T>(string cacheKey, Task<T> task)
        {
            T result = await task.ConfigureAwait(false);
            int ExpireSeconds = int.Parse(_configuration["Cache:ExpireSeconds"].ToString());
            await RedisHelper.SetAsync(cacheKey, JsonConvert.SerializeObject(result), ExpireSeconds);
            return result;
        }

        private string GenerateCacheKey(string cacheKey, IInvocation invocation)
        {
            string className = invocation.TargetType.Name;
            string methodName = invocation.Method.Name;
            List<object> methodArguments = invocation.Arguments.ToList();
            string param = string.Empty;
            if (methodArguments.Count > 0)
            {
                string serializeString = JsonConvert.SerializeObject(methodArguments, Formatting.Indented, new JsonSerializerSettings
                {
                    DefaultValueHandling = DefaultValueHandling.Ignore
                });
                param = ":" + EncryptUtil.Encrypt(serializeString);
            }
            return string.Concat(cacheKey ?? $"{className}:{methodName}", param);
        }

    }


}
