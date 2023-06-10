using IGeekFan.FreeKit.Extras;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Threading.Tasks;
using LinCms.Common;
using Newtonsoft.Json.Serialization;

namespace LinCms.Middleware;

public class AopCacheableActionFilter : IAsyncActionFilter
{
    private readonly int _expireSeconds;
    public AopCacheableActionFilter(IConfiguration configuration)
    {
        _expireSeconds = int.Parse(configuration["Cache:ExpireSeconds"].ToString());
    }

    /// <summary>
    /// 尝试从方法中获取当前的工作单元配置
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    private CacheableAttribute? TryGetCacheable(ActionExecutingContext context)
    {
        ControllerActionDescriptor? descriptor = context.ActionDescriptor as ControllerActionDescriptor;
        MethodInfo method = descriptor?.MethodInfo ?? throw new ArgumentNullException("context");
        var cacheableAttribute = method.GetCacheableAttributeOrNull();

        return cacheableAttribute;
    }

    /// <summary>
    /// 方法执行前执行UnitOfWorkManager工作单元
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!(context.ActionDescriptor is ControllerActionDescriptor))
        {
            await next();
            return;
        }

        var cacheAttr = TryGetCacheable(context);
        if (cacheAttr == null)
        {
            await next();
            return;
        }

        string cacheKey = GenerateCacheKey(cacheAttr.CacheKey, context);
        string cacheValue = await RedisHelper.GetAsync(cacheKey);
        if (cacheValue != null)
        {
            context.Result = new JsonResult(JsonConvert.DeserializeObject(cacheValue));
            return;
        }
        ActionExecutedContext result = await next();

        if (result.Exception == null || result.ExceptionHandled)
        {
            if (result.Result is ObjectResult ob)
            {
                DefaultContractResolver contractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                };

                await RedisHelper.SetAsync(cacheKey, JsonConvert.SerializeObject(ob.Value, new JsonSerializerSettings()
                {
                    ContractResolver = contractResolver
                }), _expireSeconds);
            }

        }
    }

    private string GenerateCacheKey(string cacheKey, ActionExecutingContext context)
    {
        string className = ((ControllerActionDescriptor)context.ActionDescriptor).ControllerTypeInfo.Name;
        string methodName = ((ControllerActionDescriptor)context.ActionDescriptor).ActionName;
        var methodArguments = context.ActionArguments;
        string param = string.Empty;
        if (methodArguments.Count > 0)
        {
            string serializeString = JsonConvert.SerializeObject(methodArguments, Formatting.Indented, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            });

            if (string.IsNullOrEmpty(serializeString))
            {
                param = EncryptUtil.Encrypt($"{className}:{methodName}");
            }
            else
            {
                param = ":" + EncryptUtil.Encrypt(serializeString);
            }
        }
        return string.Concat(cacheKey ?? $"{className}:{methodName}", param);
    }
}