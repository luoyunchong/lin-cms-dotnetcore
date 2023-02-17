using Castle.DynamicProxy;
using IGeekFan.FreeKit.Extras;
using LinCms.Common;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LinCms.Middleware;

/// <summary>
/// 异步事务方法拦截
/// </summary>
public class AopCacheAsyncIntercept : IAsyncInterceptor
{

    private readonly IConfiguration _configuration;
    private readonly int _expireSeconds;
    public AopCacheAsyncIntercept(IConfiguration configuration)
    {
        _configuration = configuration;
        _expireSeconds = int.Parse(_configuration["Cache:ExpireSeconds"].ToString());
    }

    /// <summary>
    /// 当只配置特性标签，但未指定任意属性值时，默认根据UnitOfWorkDefaultOptions
    /// </summary>
    /// <param name="invocation"></param>
    /// <returns></returns>
    private bool TryBegin(IInvocation invocation)
    {
        bool isEnable = bool.Parse(_configuration["Cache:Enable"]);
        Type returnType = invocation.Method.ReturnType;

        if (isEnable == false || returnType == typeof(void))
        {
            return false;
        }
        var method = invocation.MethodInvocationTarget ?? invocation.Method;
        var cacheable = method.GetCacheableAttributeOrNull();
        if (cacheable != null)
        {
            return true;
        }
        return false;
    }

    #region 拦截同步执行的方法
    /// <summary>
    /// 拦截同步执行的方法
    /// </summary>
    /// <param name="invocation"></param>
    public void InterceptSynchronous(IInvocation invocation)
    {
        if (TryBegin(invocation))
        {
            MethodInfo method = invocation.MethodInvocationTarget ?? invocation.Method;
            var cacheAttr = method.GetCustomAttributes(typeof(CacheableAttribute), false).FirstOrDefault() as CacheableAttribute;
            string cacheKey = GenerateCacheKey(cacheAttr.CacheKey, invocation);
            string cacheValue = RedisHelper.Get(cacheKey);
            if (cacheValue != null)
            {
                Type returnType = invocation.Method.ReturnType;
                invocation.ReturnValue = JsonConvert.DeserializeObject(cacheValue, returnType);
            }
            else
            {
                invocation.Proceed();
                RedisHelper.Set(cacheKey, JsonConvert.SerializeObject(invocation.ReturnValue), _expireSeconds);
            }
        }
        else
        {
            invocation.Proceed();
        }
    }
    #endregion

    #region 拦截返回结果为Task的方法
    /// <summary>
    /// 拦截返回结果为Task的方法
    /// </summary>
    /// <param name="invocation"></param>
    public void InterceptAsynchronous(IInvocation invocation)
    {
        invocation.ReturnValue = InternalInterceptAsynchronous(invocation);
    }

    private async Task InternalInterceptAsynchronous(IInvocation invocation)
    {
        if (TryBegin(invocation))
        {
            MethodInfo method = invocation.MethodInvocationTarget ?? invocation.Method;
            var cacheAttr = method.GetCustomAttributes(typeof(CacheableAttribute), false).FirstOrDefault() as CacheableAttribute;
            string cacheKey = GenerateCacheKey(cacheAttr.CacheKey, invocation);
            string cacheValue = await RedisHelper.GetAsync(cacheKey);
            if (cacheValue != null)
            {
                await InterceptAsync(cacheKey, (Task)invocation.ReturnValue);
            }
            else
            {
                await InterceptAsync(cacheKey, (dynamic)invocation.ReturnValue);
            }
        }
        else
        {
            invocation.Proceed();
        }
    }

    #endregion

    #region 拦截返回结果为Task<TResult>的方法
    /// <summary>
    /// 拦截返回结果为Task&lt;TResult&gt;的方法
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="invocation"></param>
    public void InterceptAsynchronous<TResult>(IInvocation invocation)
    {
        if (TryBegin(invocation))
        {
            MethodInfo method = invocation.MethodInvocationTarget ?? invocation.Method;
            var cacheAttr = method.GetCustomAttributes(typeof(CacheableAttribute), false).FirstOrDefault() as CacheableAttribute;
            string cacheKey = GenerateCacheKey(cacheAttr.CacheKey, invocation);
            string cacheValue = RedisHelper.Get(cacheKey);
            if (cacheValue != null)
            {
                Type returnType = invocation.Method.ReturnType;
                //将当前获取到的缓存值，赋值给当前执行方法
                Type[] resultTypes = returnType.GenericTypeArguments;
                dynamic d = JsonConvert.DeserializeObject(cacheValue, resultTypes.First())!;
                invocation.ReturnValue = Task.FromResult(d);
            }
            else
            {
                invocation.Proceed();
                invocation.ReturnValue = InterceptAsync(cacheKey, (dynamic)invocation.ReturnValue);
            }
        }
        else
        {
            invocation.Proceed();
        }
    }
    #endregion

    // do the continuation work for Task...
    private async Task InterceptAsync(string cacheKey, Task task)
    {
        await task.ConfigureAwait(false);
    }

    // do the continuation work for Task<T>...
    private async Task<T> InterceptAsync<T>(string cacheKey, Task<T> task)
    {
        T result = await task.ConfigureAwait(false);
        await RedisHelper.SetAsync(cacheKey, JsonConvert.SerializeObject(result), _expireSeconds);
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
