using Autofac;
using Autofac.Extensions.DependencyInjection;
using HealthChecks.UI.Client;
using IGeekFan.AspNetCore.Knife4jUI;
using IGeekFan.AspNetCore.RapiDoc;
using IGeekFan.FreeKit.Extras.Dependency;
using LinCms.Cms.Users;
using LinCms.Middleware;
using LinCms.Plugins.Poem.Services;
using LinCms.Startup;
using LinCms.Startup.Configuration;
using LinCms.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
ConfigurationManager c = builder.Configuration;
IServiceCollection services = builder.Services;

builder.Host
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureAppConfiguration((context, builder) =>
    {
        builder.AddJsonFile($"RateLimitConfig.json", optional: true, reloadOnChange: true);
    })
    .UseSerilog()
    .ConfigureContainer<ContainerBuilder>((webBuilder, containerBuilder) =>
    {
        containerBuilder.RegisterModule(new RepositoryModule());
        containerBuilder.RegisterModule(new ServiceModule());
        Assembly[] currentAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(r => r.FullName != null && r.FullName.Contains("LinCms.")).ToArray();
        containerBuilder.RegisterModule(new FreeKitModule(currentAssemblies));
        List<Type> interceptorServiceTypes = new List<Type>()
        {
            typeof(AopCacheIntercept)
        };
        containerBuilder.RegisterModule(new UnitOfWorkModule(currentAssemblies, interceptorServiceTypes));
        containerBuilder.RegisterModule(new AutofacModule(c));
    });

builder.WebHost.ConfigureKestrel((context, options) =>
{
    //设置应用服务器Kestrel请求体最大为50MB，默认为28.6MB
    options.Limits.MaxRequestBodySize = 1024 * 1024 * 50;
});

services
    .AddFreeSql(c)
    .AddLinServices(c)
    .AddCustomMvc(c)
    .AddAutoMapper(typeof(UserProfile).Assembly, typeof(PoemProfile).Assembly)
    .AddRedisClient(c)
    .AddJwtBearer(c)
    .AddSwaggerGen()//Swagger 扩展方法配置
    .AddCap(c)// 分布式事务一致性CAP
    .AddIpRateLimiting(c)//之前请注入AddCsRedisCore，内部实现IDistributedCache接口
    .AddGooglereCaptchav3(c)//配置Google验证码
    ;

services.AddHealthChecks();//健康检查

var app = builder.Build();

await app.Services.RunScopeClientPolicy();
app.Services.RunFreeSqlSyncStructure();

app
.UseForwardedHeaders()
.UseBasicAuthentication()
.UseHttpsRedirection()
.UseStaticFiles()
.UseSerilogRequestLogging(opts =>
{
    opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest;
    opts.GetLevel = (ctx, _, ex) =>
    {
        var path = ctx.Request.Path;
        switch (path)
        {
            case "/health":
            case "/cms/log/serilog":
                return LogEventLevel.Debug;
        }
        return ex != null || ctx.Response.StatusCode > 499 ? LogEventLevel.Error : LogEventLevel.Information;
    };
});

//异常中间件应放在MVC执行事务的中件间的前面，否则异常时UnitOfWorkMiddleware无法catch异常
//app.UseMiddleware(typeof(CustomExceptionMiddleWare));

#region 三种Swagger
app.UseSwagger();

app.UseSwaggerUI(r =>
{
    //http://localhost:5000/swagger/index.html
    r.SwaggerEndpoint("/swagger/base/swagger.json", "base");
    r.SwaggerEndpoint("/swagger/blog/swagger.json", "blog");
    r.SwaggerEndpoint("/swagger/cms/swagger.json", "cms");
    r.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    r.RoutePrefix = "s";//http://localhost:5000/s/index.html
    r.OAuthClientId(c["Service:ClientId"]);
    r.OAuthClientSecret(c["Service:ClientSecret"]);
    r.OAuthAppName(c["Service:Name"]);
    r.ConfigObject.DisplayOperationId = true;

});

app.UseKnife4UI(r =>
{
    r.DocumentTitle = "LinCms博客模块";
    r.RoutePrefix = "swagger";//http://localhost:5000/swagger/index.html
                              //r.InjectStylesheet("");
    r.SwaggerEndpoint("/base/swagger.json", "base");
    r.SwaggerEndpoint("/blog/swagger.json", "blog");
    r.SwaggerEndpoint("/cms/swagger.json", "cms");
    r.SwaggerEndpoint("/v1/swagger.json", "v1");
    r.OAuthClientSecret(c["Service:ClientSecret"]);
    r.OAuthClientId(c["Service:ClientId"]);
    r.OAuthAppName(c["Service:Name"]);
});

app.UseRapiDocUI(r =>
{
    r.RoutePrefix = ""; //RapiDoc http://localhost:5000/index.html
    r.SwaggerEndpoint("/swagger/base/swagger.json", "base");
    r.SwaggerEndpoint("/swagger/blog/swagger.json", "blog");
    r.SwaggerEndpoint("/swagger/cms/swagger.json", "cms");
    r.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    r.GenericRapiConfig = new GenericRapiConfig()
    {
        RenderStyle = "focused",//read | view | focused
        Theme = "light",//light,dark,focused
        SchemaStyle = "table",//tree | table
        ShowMethodInNavBar = "as-colored-text"
    };

});

#endregion

app.UseCors(policyName: "CorsPolicy");

//认证中间件
app.UseAuthentication();

//IP 限流 RateLimitConfig.json
app.UseMiddleware<IpLimitMiddleware>();

//Fix login issue Exception: Correlation failed
//https://github.com/dotnet-architecture/eShopOnContainers/pull/1516
app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });

app.UseRouting()
    .UseAuthorization()
    .UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();

        endpoints.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = s => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
    });

app.Run();