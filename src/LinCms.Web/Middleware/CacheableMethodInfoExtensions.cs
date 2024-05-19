using System.Linq;
using System.Reflection;
using LinCms.Aop.Attributes;

namespace LinCms.Middleware;

public static class CacheableMethodInfoExtensions
{
    /// <summary>
    /// 判断方法上或类上是否有CacheableAttribute特性标签 
    /// </summary>
    /// <param name="methodInfo"></param>
    /// <returns></returns>
    public static CacheableAttribute? GetCacheableAttributeOrNull(this MethodInfo methodInfo)
    {
        var attrs = methodInfo.GetCustomAttributes(true).OfType<CacheableAttribute>().ToArray();
        if (attrs.Length > 0)
        {
            return attrs[0];
        }

        if (methodInfo.DeclaringType == null) return null;
        attrs = methodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes(true).OfType<CacheableAttribute>().ToArray();
        if (attrs.Length > 0)
        {
            return attrs[0];
        }

        return null;
    }
}
