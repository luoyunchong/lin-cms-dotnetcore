using System;
using System.Threading;
using LinCms.Core.Security;
using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace LinCms.Web.Utils
{
    public class LogHelper
    {
        public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            var request = httpContext.Request;
            ICurrentUser currentUser = (ICurrentUser)httpContext.RequestServices.GetService(typeof(ICurrentUser));
            if (currentUser != null)
            {
                diagnosticContext.Set("UserName", currentUser.UserName);
                diagnosticContext.Set("UserId", currentUser.Id);
            }
            // et all the common properties available for every request
            diagnosticContext.Set("Host", request.Host);
            diagnosticContext.Set("Protocol", request.Protocol);
            diagnosticContext.Set("Scheme", request.Scheme);

            // Only set it if available. You're not sending sensitive data in a querystring right?!
            if (request.QueryString.HasValue)
            {
                diagnosticContext.Set("QueryString", request.QueryString.Value);
            }

            // Set the content-type of the Response at this point
            diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

            // Retrieve the IEndpointFeature selected for the request
            var endpoint = httpContext.GetEndpoint();
            if (endpoint != null)
            {
                diagnosticContext.Set("EndpointName", endpoint.DisplayName);
            }
        }

        public static LogEventLevel ExcludeHealthChecks(HttpContext ctx, double _, Exception ex)
        {
            return ex != null
                     ? LogEventLevel.Error
                     : ctx.Response.StatusCode > 499
                         ? LogEventLevel.Error
                         : IsHealthCheckEndpoint(ctx) // Not an error, check if it was a health check
                             ? LogEventLevel.Verbose // Was a health check, use Verbose
                             : LogEventLevel.Information;
        }

        private static bool IsHealthCheckEndpoint(HttpContext ctx)
        {
            var endpoint = ctx.GetEndpoint();
            if (endpoint is object) // same as !(endpoint is null)
            {
                return string.Equals(
                    endpoint.DisplayName,
                    "Health checks",
                    StringComparison.Ordinal);
            }
            // No endpoint, so not a health check endpoint
            return false;
        }
    }
}