using System;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using AspNet.Security.OAuth.Gitee;
using DotNetCore.Security;
using LinCms.Common;
using LinCms.Data;
using LinCms.Data.Authorization;
using LinCms.Data.Enums;
using LinCms.IRepositories;
using LinCms.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace LinCms.Startup;

public static class JwtExtensions
{
    public static JwtSettings AddSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        string? signingKey = configuration["Authentication:JwtBearer:SecurityKey"];
        if (string.IsNullOrWhiteSpace(signingKey))
        {
            signingKey = LoadOrCreateLocalSigningKey();
        }
        else if (signingKey.Length < 64 ||
            signingKey.StartsWith("lin-cms-dotnetcore-", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Authentication__JwtBearer__SecurityKey must contain at least 64 characters and must not use a former repository default.");
        }

        JwtSettings jsonWebTokenSettings = new JwtSettings(
            signingKey,
            new TimeSpan(10, 0, 0, 0),
            configuration["Authentication:JwtBearer:Audience"],
            configuration["Authentication:JwtBearer:Issuer"]
        );
        services.AddHashService();
        services.AddICryptographyService("lin-cms-dotnetcore-cryptography");
        services.AddJwtService(jsonWebTokenSettings);
        return jsonWebTokenSettings;
    }

    private static string LoadOrCreateLocalSigningKey()
    {
        string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
            ?? "Development";
        string fileName = environment.Equals("Production", StringComparison.OrdinalIgnoreCase)
            ? "appsettings.Production.json"
            : "appsettings.json";
        string path = Environment.GetEnvironmentVariable("LINCMS_APPSETTINGS_FILE")
            ?? Path.Combine(AppContext.BaseDirectory, fileName);

        try
        {
            if (File.Exists(path))
            {
                string existingKey = File.ReadAllText(path).Trim();
                if (existingKey.Length >= 64 && !existingKey.StartsWith("lin-cms-dotnetcore-", StringComparison.OrdinalIgnoreCase))
                {
                    return existingKey;
                }
            }

            string generatedKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            string settings = File.ReadAllText(path);
            string updatedSettings = new Regex(
                @"(""SecurityKey""\s*:\s*)""[^""]*""",
                RegexOptions.None)
                .Replace(settings, $"$1\"{generatedKey}\"", 1);
            if (ReferenceEquals(settings, updatedSettings) || settings == updatedSettings)
            {
                throw new InvalidOperationException($"The JWT SecurityKey setting was not found in '{path}'.");
            }
            string temporaryPath = $"{path}.{Environment.ProcessId}.tmp";
            File.WriteAllText(temporaryPath, updatedSettings, new UTF8Encoding(false));
            File.Move(temporaryPath, path, true);
            Console.Error.WriteLine($"Authentication:JwtBearer:SecurityKey was not configured. Generated and persisted a key in '{path}'.");
            return generatedKey;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            throw new InvalidOperationException($"JWT signing key is missing and could not be persisted to '{path}'. Configure Authentication__JwtBearer__SecurityKey or grant write access to the key file location.", exception);
        }
    }

    public static IServiceCollection AddJwtBearer(this IServiceCollection services, IConfiguration Configuration)
    {
        JwtSettings jsonWebTokenSettings = services.AddSecurity(Configuration);

        //基于策略 处理 退出登录 黑名单策略 授权
        services.AddAuthorization(options =>
        {
            var defaultPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .AddRequirements(new ValidJtiRequirement())
                .Build();
            options.AddPolicy("Bearer", defaultPolicy);
            // If no policy specified, use this
            options.DefaultPolicy = defaultPolicy;
        });

        services.Configure<BasicAuthenticationOption>(Configuration.GetSection("Basic"));
        BasicAuthenticationOption basicOption = new BasicAuthenticationOption();
        Configuration.Bind("Basic", basicOption);

        //认证
        AuthenticationBuilder authenticationBuilder = services
             .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)//使用指定的方案启用 JWT 持有者身份验证。
             .AddCookie()
             .AddJwtBearer(options =>
             {
                 options.RequireHttpsMetadata = Configuration["Service:UseHttps"].ToBoolean();
                 options.Audience = Configuration["Service:Name"];

                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     // The signing key must match!
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = jsonWebTokenSettings.SecurityKey,

                     // Validate the JWT Issuer (iss) claim
                     ValidateIssuer = true,
                     ValidIssuer = jsonWebTokenSettings.Issuer,

                     // Validate the JWT Audience (aud) claim
                     ValidateAudience = true,
                     ValidAudience = jsonWebTokenSettings.Audience,

                     // Validate the token expiry
                     ValidateLifetime = true,

                     // If you want to allow a certain amount of clock drift, set thatValidIssuer  here
                     //ClockSkew = TimeSpan.Zero
                 };
                 //使用Authorize设置为需要登录时，返回json格式数据。
                 options.Events = new JwtBearerEvents()
                 {
                     OnTokenValidated = async context =>
                     {
                         ClaimsIdentity? identity = context.Principal?.Identity as ClaimsIdentity;
                         if (identity == null || !long.TryParse(identity.FindFirst(ClaimTypes.NameIdentifier)?.Value, out long userId) || userId <= 0)
                         {
                             context.Fail("Invalid user identity.");
                             return;
                         }

                         var repository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
                         var user = await repository.GetUserAsync(r => r.Id == userId && !r.IsDeleted);
                         if (user == null || !user.IsActive())
                         {
                             context.Fail("The user account is unavailable.");
                             return;
                         }

                         // Authorization always uses the current server-side groups.
                         foreach (var claim in identity.Claims.Where(c => c.Type == identity.RoleClaimType || c.Type == "role" || c.Type == LinCmsClaimTypes.GroupIds).ToList())
                         {
                             identity.RemoveClaim(claim);
                         }
                         foreach (var group in user.LinGroups ?? Array.Empty<LinCms.Entities.LinGroup>())
                         {
                             identity.AddClaim(new Claim(identity.RoleClaimType, group.Name));
                             identity.AddClaim(new Claim(LinCmsClaimTypes.GroupIds, group.Id.ToString()));
                         }
                     },
                     OnAuthenticationFailed = context =>
                     {
                         //Token expired
                         if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                         {
                             context.Response.Headers.Add("Token-Expired", "true");
                         }

                         return Task.CompletedTask;
                     },
                     OnChallenge = async context =>
                     {
                         //此处代码为终止.Net Core默认的返回类型和数据结果，这个很重要哦
                         context.HandleResponse();

                         string message;
                         ErrorCode errorCode;
                         int statusCode = StatusCodes.Status401Unauthorized;

                         if (context.Error == "invalid_token" &&
                             context.ErrorDescription == "The token is expired")
                         {
                             message = "令牌过期";
                             errorCode = ErrorCode.TokenExpired;
                             statusCode = StatusCodes.Status422UnprocessableEntity;
                         }
                         else if (context.Error == "invalid_token" && context.ErrorDescription.IsNullOrEmpty())
                         {
                             message = "令牌失效";
                             errorCode = ErrorCode.TokenInvalidation;
                         }
                         else
                         {
                             message = "请先登录 " + context.ErrorDescription; //""认证失败，请检查请求头或者重新登录";
                             errorCode = ErrorCode.AuthenticationFailed;
                         }

                         context.Response.ContentType = "application/json";
                         context.Response.StatusCode = statusCode;
                         await context.Response.WriteAsync(new UnifyResponseDto(errorCode, message, context.HttpContext).ToString());

                     }
                 };
             });

        if (Configuration["Authentication:GitHub:Enable"] != null && bool.Parse(Configuration["Authentication:GitHub:Enable"]!))
        {
            authenticationBuilder.AddGitHub(options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ClientId = Configuration["Authentication:GitHub:ClientId"]!;
                options.ClientSecret = Configuration["Authentication:GitHub:ClientSecret"]!;
                options.Scope.Add("user:email");
                options.ClaimActions.MapJsonKey(LinConsts.Claims.AvatarUrl, "avatar_url");
                options.ClaimActions.MapJsonKey(LinConsts.Claims.HtmlUrl, "html_url");
                //登录成功后可通过  authenticateResult.Principal.FindFirst(ClaimTypes.Uri)?.Value;  得到GitHub头像
                options.ClaimActions.MapJsonKey(LinConsts.Claims.Bio, "bio");
                options.ClaimActions.MapJsonKey(LinConsts.Claims.BlogAddress, "blog");
            });
        }
     
        if (Configuration["Authentication:GitHub:Enable"] != null && bool.Parse(Configuration["Authentication:GitHub:Enable"]!))
        {
            authenticationBuilder.AddGitee(GiteeAuthenticationDefaults.AuthenticationScheme, "码云", options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ClientId = Configuration["Authentication:Gitee:ClientId"]!;
                options.ClientSecret = Configuration["Authentication:Gitee:ClientSecret"]!;

                options.ClaimActions.MapJsonKey("urn:gitee:avatar_url", "avatar_url");
                options.ClaimActions.MapJsonKey("urn:gitee:blog", "blog");
                options.ClaimActions.MapJsonKey("urn:gitee:bio", "bio");
                options.ClaimActions.MapJsonKey("urn:gitee:html_url", "html_url");
                //options.Scope.Add("projects");
                //options.Scope.Add("pull_requests");
                //options.Scope.Add("issues");
                //options.Scope.Add("notes");
                //options.Scope.Add("keys");
                //options.Scope.Add("hook");
                //options.Scope.Add("groups");
                //options.Scope.Add("gists");
                //options.Scope.Add("enterprises");

                options.SaveTokens = true;
            });
        }
      

        authenticationBuilder.AddScheme<BasicAuthenticationOption, BasicAuthenticationHandler>(BasicAuthenticationScheme.DefaultScheme, r =>
            {
                r.UserName = basicOption.UserName;
                r.UserPassword = basicOption.UserPassword;
                r.Realm = basicOption.Realm;
            });

        return services;
    }
}
