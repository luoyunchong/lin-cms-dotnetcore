using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public static class BasicAuthenticationScheme
{
    public const string DefaultScheme = "Basic";
}
public class BasicAuthenticationOption : AuthenticationSchemeOptions
{
    public bool Enable { get; set; }
    public List<string> ProtectPaths { get; set; }
    /// <summary>
    /// 指明HTTP基本认证的是这个资源集
    /// </summary>
    public string Realm { get; set; }
    public string UserName { get; set; }
    public string UserPassword { get; set; }
}

public class BasicAuthenticationHandler : AuthenticationHandler<BasicAuthenticationOption>
{
    public BasicAuthenticationHandler(
        IOptionsMonitor<BasicAuthenticationOption> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    /// <summary>
    /// 认证
    /// </summary>
    /// <returns></returns>
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
            return AuthenticateResult.Fail("Missing Authorization Header");
        string username, password;
        try
        {
            AuthenticationHeaderValue authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            if (authHeader.Parameter == null)
            {
                return AuthenticateResult.Fail("Authorization Header is null");
            }
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
            username = credentials[0];
            password = credentials[1];
            var isValidUser = IsAuthorized(username, password);
            if (isValidUser == false)
            {
                return AuthenticateResult.Fail("Invalid username or password");
            }
        }
        catch
        {
            return AuthenticateResult.Fail("Invalid Authorization Header");
        }

        var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier,username),
                new Claim(ClaimTypes.Name,username),
            };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return await Task.FromResult(AuthenticateResult.Success(ticket));
    }

    /// <summary>
    /// 质询
    /// </summary>
    /// <param name="properties"></param>
    /// <returns></returns>
    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{Options.Realm}\"";
        await base.HandleChallengeAsync(properties);
    }

    /// <summary>
    /// 认证失败
    /// </summary>
    /// <param name="properties"></param>
    /// <returns></returns>
    protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        await base.HandleForbiddenAsync(properties);
    }

    private bool IsAuthorized(string username, string password)
    {
        return username.Equals(base.Options.UserName, StringComparison.InvariantCultureIgnoreCase)
               && password.Equals(base.Options.UserPassword);
    }
}

// HTTP基本认证Middleware
public static class BasicAuthentication
{
    public static IApplicationBuilder UseBasicAuthentication(this IApplicationBuilder app)
    {
        IOptionsMonitor<BasicAuthenticationOption>? optionsMonitor = app.ApplicationServices.GetService<IOptionsMonitor<BasicAuthenticationOption>>();

        if (optionsMonitor != null && optionsMonitor.CurrentValue.ProtectPaths.Count > 0 && optionsMonitor.CurrentValue.Enable)
        {
            Expression<Func<HttpContext, bool>> expression = u => false;
            foreach (var path in optionsMonitor.CurrentValue.ProtectPaths)
            {
                expression = expression.Or(x => x.Request.Path.StartsWithSegments(new PathString(path)));
            }
            app.UseWhen(expression.Compile(), configuration: x => { x.UseMiddleware<BasicAuthenticationMiddleware>(); });
        }

        return app;
    }
}

public class BasicAuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public BasicAuthenticationMiddleware(RequestDelegate next, ILoggerFactory LoggerFactory)
    {
        _next = next;
        _logger = LoggerFactory.CreateLogger<BasicAuthenticationMiddleware>();
    }
    public async Task Invoke(HttpContext httpContext, IAuthenticationService authenticationService)
    {
        var authenticated = await authenticationService.AuthenticateAsync(httpContext, BasicAuthenticationScheme.DefaultScheme);
        _logger.LogInformation("Access Status：" + authenticated.Succeeded);
        if (!authenticated.Succeeded)
        {
            await authenticationService.ChallengeAsync(httpContext, BasicAuthenticationScheme.DefaultScheme, new AuthenticationProperties { });
            return;
        }
        await _next(httpContext);
    }
}