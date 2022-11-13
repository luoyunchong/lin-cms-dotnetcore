using System;
using System.Threading.Tasks;
using AspNet.Security.OAuth.Gitee;
using DotNetCore.Security;
using LinCms.Common;
using LinCms.Data;
using LinCms.Data.Authorization;
using LinCms.Data.Enums;
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
        JwtSettings jsonWebTokenSettings = new JwtSettings(
            configuration["Authentication:JwtBearer:SecurityKey"],
            new TimeSpan(1, 0, 0, 0),
            configuration["Authentication:JwtBearer:Audience"],
            configuration["Authentication:JwtBearer:Issuer"]
        );
        services.AddHashService();
        services.AddICryptographyService("lin-cms-dotnetcore-cryptography");
        services.AddJwtService(jsonWebTokenSettings);
        return jsonWebTokenSettings;
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
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)//使用指定的方案启用 JWT 持有者身份验证。
            .AddCookie()
            .AddJwtBearer(options =>
            {
                bool isIds4 = Configuration["Service:IdentityServer4"].ToBoolean();

                if (isIds4)
                {
                    //identityserver4 地址 也就是本项目地址
                    options.Authority = Configuration["Service:Authority"];
                }
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

                //options.TokenValidationParameters = new TokenValidationParameters()
                //{
                //    ClockSkew = TimeSpan.Zero   //偏移设置为了0s,用于测试过期策略,完全按照access_token的过期时间策略，默认原本为5分钟
                //};


                //使用Authorize设置为需要登录时，返回json格式数据。
                options.Events = new JwtBearerEvents()
                {
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
            })
            .AddGitHub(options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ClientId = Configuration["Authentication:GitHub:ClientId"];
                options.ClientSecret = Configuration["Authentication:GitHub:ClientSecret"];
                options.Scope.Add("user:email");
                options.ClaimActions.MapJsonKey(LinConsts.Claims.AvatarUrl, "avatar_url");
                options.ClaimActions.MapJsonKey(LinConsts.Claims.HtmlUrl, "html_url");
                //登录成功后可通过  authenticateResult.Principal.FindFirst(ClaimTypes.Uri)?.Value;  得到GitHub头像
                options.ClaimActions.MapJsonKey(LinConsts.Claims.Bio, "bio");
                options.ClaimActions.MapJsonKey(LinConsts.Claims.BlogAddress, "blog");
            })
            .AddQQ(options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ClientId = Configuration["Authentication:QQ:ClientId"];
                options.ClientSecret = Configuration["Authentication:QQ:ClientSecret"];
            })
            .AddGitee(GiteeAuthenticationDefaults.AuthenticationScheme, "码云", options =>
            {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.ClientId = Configuration["Authentication:Gitee:ClientId"];
                options.ClientSecret = Configuration["Authentication:Gitee:ClientSecret"];

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
            })
            .AddScheme<BasicAuthenticationOption, BasicAuthenticationHandler>(BasicAuthenticationScheme.DefaultScheme, r =>
            {
                r.UserName = basicOption.UserName;
                r.UserPassword = basicOption.UserPassword;
                r.Realm = basicOption.Realm;
            });

        return services;
    }
}