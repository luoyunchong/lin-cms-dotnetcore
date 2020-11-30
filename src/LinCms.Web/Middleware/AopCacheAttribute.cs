using Castle.DynamicProxy;
using CSRedis;
using LinCms;
using LinCms.Aop;
using LinCms.Aop.Attributes;
using LinCms.Middleware;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LinCms.Middleware
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AopCacheAttribute : Attribute
    {
        public AopCacheAttribute()
        {
        }

        public AopCacheAttribute(string cacheKey)
        {
            CacheKey = cacheKey;
        }

        public string CacheKey { get; set; }


    }
}
