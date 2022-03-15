using System.Diagnostics;
using IGeekFan.AspNetCore.RapiDoc;
using LinCms.IdentityServer4;
using LinCms.IdentityServer4.IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

#region SerilogÅäÖÃ

builder.Host.UseSerilog();
#if !DEBUG
//.UseKestrel((context, options) =>
//     {
//         options.Configure(context.Configuration.GetSection("Kestrel"));
//     }) 
#endif

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration)
       .Enrich.FromLogContext()
       .CreateLogger();
   Log.Information("Starting web host");
#if DEBUG
Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
#endif

#endregion

InMemoryConfiguration.Configuration = Configuration;

builder.Services.AddServices(Configuration);

var app = builder.Build();


var options = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
options.KnownNetworks.Clear();
options.KnownProxies.Clear();
app.UseForwardedHeaders(options);

//app.UseMiddleware(typeof(CustomExceptionMiddleWare));
app.UseHttpsRedirection();

//app.UseSerilogRequestLogging();

app.UseCors(builder =>
{
    string[] withOrigins = Configuration.GetSection("WithOrigins").Get<string[]>();
    builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins(withOrigins);
});

app.UseCookiePolicy();
app.UseSession();

app.UseRouting();
app.UseStaticFiles();

app.UseAuthorization();

app.UseIdentityServer();

app.UseSwagger();

app.UseRapiDocUI(c =>
{
    c.DocumentTitle = "LinCms.IdentityServer4";
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "LinCms.IdentityServer4");
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHealthChecks("/health");
});

app.Run();