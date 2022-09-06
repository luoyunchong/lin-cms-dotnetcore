using IGeekFan.FreeKit.Extras.Security;
using LinCms.Security;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace LinCms.Utils;

public class LogHelper
{
    public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
    {
        var request = httpContext.Request;
        ICurrentUser? currentUser = httpContext.RequestServices.GetService(typeof(ICurrentUser)) as ICurrentUser;
        if (currentUser != null)
        {
            diagnosticContext.Set("UserName", currentUser.UserName);
            diagnosticContext.Set("UserId", currentUser.FindUserId());
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
}