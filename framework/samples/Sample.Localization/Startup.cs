using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FreeSql;
using IGeekFan.Localization.FreeSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Sample.Localization
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            IConfigurationSection Mysql = Configuration.GetSection("Mysql");
            Fsql = new FreeSqlBuilder()
                .UseConnectionString(DataType.MySql, Mysql.Value)
                .UseAutoSyncStructure(true)
                .UseMonitorCommand(cmd => Trace.WriteLine(cmd.CommandText))
                .Build().SetDbContextOptions(options => { options.EnableAddOrUpdateNavigateList = true; });
        }
        public IFreeSql Fsql { get; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton(Fsql);
            services.AddSingleton<IStringLocalizerFactory, FreeSqlStringLocalizerFactory>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IStringLocalizerFactory localizerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en-US"),
                new CultureInfo("ja-JP"),
                new CultureInfo("fr-FR"),
                new CultureInfo("zh-CN")
            };
            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };
            app.UseRequestLocalization(options);

            var SR = localizerFactory.Create(null);

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/index", async context =>
                {
                    if (context.Request.Path.Value.EndsWith("favicon.ico"))
                    {
                        // Pesky browsers
                        context.Response.StatusCode = 404;
                        return;
                    }
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "text/html; charset=utf-8";

                    var requestCultureFeature = context.Features.Get<IRequestCultureFeature>();
                    var requestCulture = requestCultureFeature.RequestCulture;

                    await context.Response.WriteAsync(
                      $@"<!doctype html>
                        <html>
                        <head>
                            <title>{SR["Request Localization"]}</title>
                            <style>
                                body {{ font-family: 'Segoe UI', Helvetica, Sans-Serif }}
                                h1, h2, h3, h4, th {{ font-family: 'Segoe UI Light', Helvetica, Sans-Serif }}
                                th {{ text-align: left }}
                            </style>
                            <script>
                                function useCookie() {{
                                    var culture = document.getElementById('culture');
                                    var uiCulture = document.getElementById('uiCulture');
                                    var cookieValue = '{CookieRequestCultureProvider.DefaultCookieName}=c='+culture.options[culture.selectedIndex].value+'|uic='+uiCulture.options[uiCulture.selectedIndex].value;
                                    document.cookie = cookieValue;
                                    window.location = window.location.href.split('?')[0];
                                }}

                                function clearCookie() {{
                                    document.cookie='{CookieRequestCultureProvider.DefaultCookieName}=""""';
                                }}
                            </script>
                        </head>
                        <body>");
                    await context.Response.WriteAsync($"<h1>{SR["Request Localization Sample"]}</h1>");
                    await context.Response.WriteAsync($"<h1>{SR["Hello"]}</h1>");
                    await context.Response.WriteAsync("<form id=\"theForm\" method=\"get\">");
                    await context.Response.WriteAsync($"<label for=\"culture\">{SR["Culture"]}: </label>");
                    await context.Response.WriteAsync("<select id=\"culture\" name=\"culture\">");
                    await WriteCultureSelectOptions(context);
                    await context.Response.WriteAsync("</select><br />");
                    await context.Response.WriteAsync($"<label for=\"uiCulture\">{SR["UI Culture"]}: </label>");
                    await context.Response.WriteAsync("<select id=\"uiCulture\" name=\"ui-culture\">");
                    await WriteCultureSelectOptions(context);
                    await context.Response.WriteAsync("</select><br />");
                    await context.Response.WriteAsync("<input type=\"submit\" value=\"go QS\" /> ");
                    await context.Response.WriteAsync($"<input type=\"button\" value=\"go cookie\" onclick='useCookie();' /> ");
                    await context.Response.WriteAsync($"<a href=\"/\" onclick='clearCookie();'>{SR["reset"]}</a>");
                    await context.Response.WriteAsync("</form>");
                    await context.Response.WriteAsync("<br />");
                    await context.Response.WriteAsync("<table><tbody>");
                    await context.Response.WriteAsync($"<tr><th>Winning provider:</th><td>{requestCultureFeature.Provider.GetType().Name}</td></tr>");
                    await context.Response.WriteAsync($"<tr><th>{SR["Current request culture:"]}</th><td>{requestCulture.Culture.DisplayName} ({requestCulture.Culture})</td></tr>");
                    await context.Response.WriteAsync($"<tr><th>{SR["Current request UI culture:"]}</th><td>{requestCulture.UICulture.DisplayName} ({requestCulture.UICulture})</td></tr>");
                    await context.Response.WriteAsync($"<tr><th>{SR["Current thread culture:"]}</th><td>{CultureInfo.CurrentCulture.DisplayName} ({CultureInfo.CurrentCulture})</td></tr>");
                    await context.Response.WriteAsync($"<tr><th>{SR["Current thread UI culture:"]}</th><td>{CultureInfo.CurrentUICulture.DisplayName} ({CultureInfo.CurrentUICulture})</td></tr>");
                    await context.Response.WriteAsync($"<tr><th>{SR["Current date (invariant full):"]}</th><td>{DateTime.Now.ToString("F", CultureInfo.InvariantCulture)}</td></tr>");
                    await context.Response.WriteAsync($"<tr><th>{SR["Current date (invariant):"]}</th><td>{DateTime.Now.ToString(CultureInfo.InvariantCulture)}</td></tr>");
                    await context.Response.WriteAsync($"<tr><th>{SR["Current date (request full):"]}</th><td>{DateTime.Now.ToString("F")}</td></tr>");
                    await context.Response.WriteAsync($"<tr><th>{SR["Current date (request):"]}</th><td>{DateTime.Now.ToString()}</td></tr>");
                    await context.Response.WriteAsync($"<tr><th>{SR["Current time (invariant):"]}</th><td>{DateTime.Now.ToString("T", CultureInfo.InvariantCulture)}</td></tr>");
                    await context.Response.WriteAsync($"<tr><th>{SR["Current time (request):"]}</th><td>{DateTime.Now.ToString("T")}</td></tr>");
                    await context.Response.WriteAsync($"<tr><th>{SR["Big number (invariant):"]}</th><td>{(Math.Pow(2, 42) + 0.42).ToString("N", CultureInfo.InvariantCulture)}</td></tr>");
                    await context.Response.WriteAsync($"<tr><th>{SR["Big number (request):"]}</th><td>{(Math.Pow(2, 42) + 0.42).ToString("N")}</td></tr>");
                    await context.Response.WriteAsync($"<tr><th>{SR["Big number negative (invariant):"]}</th><td>{(-Math.Pow(2, 42) + 0.42).ToString("N", CultureInfo.InvariantCulture)}</td></tr>");
                    await context.Response.WriteAsync($"<tr><th>{SR["Big number negative (request):"]}</th><td>{(-Math.Pow(2, 42) + 0.42).ToString("N")}</td></tr>");
                    await context.Response.WriteAsync($"<tr><th>{SR["Money (invariant):"]}</th><td>{2199.50.ToString("C", CultureInfo.InvariantCulture)}</td></tr>");
                    await context.Response.WriteAsync($"<tr><th>{SR["Money (request):"]}</th><td>{2199.50.ToString("C")}</td></tr>");
                    await context.Response.WriteAsync($"<tr><th>{SR["Money negative (invariant):"]}</th><td>{(-2199.50).ToString("C", CultureInfo.InvariantCulture)}</td></tr>");
                    await context.Response.WriteAsync($"<tr><th>{SR["Money negative (request):"]}</th><td>{(-2199.50).ToString("C")}</td></tr>");
                    await context.Response.WriteAsync("</tbody></table>");
                    await context.Response.WriteAsync(
    @"</body>
</html>");
                });

            });


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        private static async Task WriteCultureSelectOptions(HttpContext context)
        {
            await context.Response.WriteAsync($"    <option value=\"\">-- select --</option>");
            await context.Response.WriteAsync($"    <option value=\"{new CultureInfo("en-US").Name}\">{new CultureInfo("en-US").DisplayName}</option>");
            await context.Response.WriteAsync($"    <option value=\"{new CultureInfo("fr-FR").Name}\">{new CultureInfo("fr-FR").DisplayName}</option>");
            await context.Response.WriteAsync($"    <option value=\"{new CultureInfo("ja-JP").Name}\">{new CultureInfo("ja-JP").DisplayName}</option>");
            await context.Response.WriteAsync($"    <option value=\"{new CultureInfo("zh-CN").Name}\">{new CultureInfo("zh-CN").DisplayName}</option>");
        }
    }
}
