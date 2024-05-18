using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using LinCms.Exceptions;
using LinCms.Models.Options;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Owl.reCAPTCHA;
using Owl.reCAPTCHA.v3;

namespace LinCms.Middleware;

/// <summary>
/// 验证码过滤器，可配置某个方法是否需要Google人机验证
/// </summary>
public class RecaptchaVerifyActionFilter(ILogger<RecaptchaVerifyActionFilter> logger,
        IOptionsMonitor<GooglereCAPTCHAOptions> options,
        IreCAPTCHASiteVerifyV3 siteVerify)
    : ActionFilterAttribute
{
    private readonly GooglereCAPTCHAOptions _options = options.CurrentValue;

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!_options.Enabled)
        {
            await next();
            return;
        }

        if (context.HttpContext.Request.Headers.ContainsKey(_options.HeaderKey))
        {
            string googleRecaptchaToken = context.HttpContext.Request.Headers[_options.HeaderKey].ToString();

            if (googleRecaptchaToken.IsNullOrEmpty())
            {
                throw new LinCmsException("验证参数不存在，人机验证失败！");
            }

            try
            {
                reCAPTCHASiteVerifyV3Response response = await siteVerify.Verify(new reCAPTCHASiteVerifyRequest
                {
                    Response = googleRecaptchaToken,
                    RemoteIp = context.HttpContext.Connection.RemoteIpAddress?.ToString()
                });

                if (!response.Success || response.Score != 0 && response.Score < _options.MinimumScore)
                {
                    throw new LinCmsException("人机验证失败！");
                }
            }
            catch (NetworkInformationException ex)
            {
                logger.LogError($"Google人机验证网络请求失败：{ex.Message}{ex.StackTrace}");
            }

            await next();
            return;
        }

        throw new LinCmsException("人机验证失败，请检查参数！");
    }
}