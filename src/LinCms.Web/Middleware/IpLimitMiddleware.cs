using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LinCms.Middleware
{
    public class IpLimitMiddleware : IpRateLimitMiddleware
    {
        public IpLimitMiddleware(RequestDelegate next, IProcessingStrategy processingStrategy, IOptions<IpRateLimitOptions> options, IRateLimitCounterStore counterStore, IIpPolicyStore policyStore, IRateLimitConfiguration config, ILogger<IpRateLimitMiddleware> logger)
            : base(next, processingStrategy, options, counterStore, policyStore, config, logger)
        {
        }

        public override Task ReturnQuotaExceededResponse(HttpContext httpContext, RateLimitRule rule, string retryAfter)
        {
            httpContext.Response.Headers.Append("Access-Control-Allow-Origin", "*");
            return base.ReturnQuotaExceededResponse(httpContext, rule, retryAfter);
        }
    }
}
