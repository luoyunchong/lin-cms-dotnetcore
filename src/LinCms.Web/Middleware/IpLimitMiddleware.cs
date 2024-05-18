using System.Threading.Tasks;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LinCms.Middleware;

public class IpLimitMiddleware(RequestDelegate next, IProcessingStrategy processingStrategy,
        IOptions<IpRateLimitOptions> options, IIpPolicyStore policyStore, IRateLimitConfiguration config,
        ILogger<IpRateLimitMiddleware> logger)
    : IpRateLimitMiddleware(next, processingStrategy, options, policyStore, config, logger)
{
    public override Task ReturnQuotaExceededResponse(HttpContext httpContext, RateLimitRule rule, string retryAfter)
    {
        httpContext.Response.Headers.Append("Access-Control-Allow-Origin", "*");
        return base.ReturnQuotaExceededResponse(httpContext, rule, retryAfter);
    }
}